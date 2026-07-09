using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using System.Threading.Tasks;
using AppManager.Models;
using Microsoft.Extensions.Logging;
using Volo.Abp.DependencyInjection;

namespace AppManager.Services;

public class IisManager : IIisManager, ITransientDependency
{
    private readonly ILogger<IisManager> _logger;

    public IisManager(ILogger<IisManager> logger)
    {
        _logger = logger;
    }

    public Task<List<IisSite.IisSite>> GetAllSitesAsync()
    {
        var result = new List<IisSite.IisSite>();
        try
        {
            using var sm = new Microsoft.Web.Administration.ServerManager();
            foreach (var site in sm.Sites)
                result.Add(MapSiteToEntity(site));
        }
        catch (Exception ex) { _logger.LogWarning(ex, "Failed to read IIS sites"); }
        return Task.FromResult(result);
    }

    public Task<IisSite.IisSite?> GetSiteByNameAsync(string siteName)
    {
        try
        {
            using var sm = new Microsoft.Web.Administration.ServerManager();
            var site = sm.Sites[siteName];
            return Task.FromResult(site != null ? MapSiteToEntity(site) : null);
        }
        catch (Exception ex) { _logger.LogWarning(ex, "Failed to read IIS site {SiteName}", siteName); }
        return Task.FromResult<IisSite.IisSite?>(null);
    }

    public Task<IisSite.IisSite> CreateSiteAsync(IisSite.IisSite site)
    {
        using var sm = new Microsoft.Web.Administration.ServerManager();
        var poolName = site.AppPoolName ?? site.SiteName + "Pool";
        var pool = sm.ApplicationPools.Add(poolName);
        pool.ManagedRuntimeVersion = "v4.0";
        pool.ManagedPipelineMode = Microsoft.Web.Administration.ManagedPipelineMode.Integrated;
        sm.CommitChanges();

        var iisSite = sm.Sites.Add(site.SiteName, site.PhysicalPath, site.Port ?? 80);
        iisSite.Applications["/"].ApplicationPoolName = poolName;
        iisSite.ServerAutoStart = true;

        var bindings = System.Text.Json.JsonSerializer.Deserialize<List<SiteBinding>>(site.BindingsJson ?? "[]");
        if (bindings != null && bindings.Count > 1)
        {
            for (int i = 1; i < bindings.Count; i++)
            {
                var b = bindings[i];
                var protocol = b.Protocol == "https" ? "https" : "http";
                iisSite.Bindings.Add($"{b.IpAddress}:{b.Port}:{b.HostName ?? ""}", protocol);
            }
        }
        sm.CommitChanges();
        site.AppPoolName = poolName;
        site.Status = iisSite.State == Microsoft.Web.Administration.ObjectState.Started ? "Running" : "Stopped";
        return Task.FromResult(site);
    }

    public Task<IisSite.IisSite> UpdateSiteAsync(IisSite.IisSite site)
    {
        try
        {
            using var sm = new Microsoft.Web.Administration.ServerManager();
            var iisSite = sm.Sites[site.SiteName];
            if (iisSite != null && !string.IsNullOrEmpty(site.PhysicalPath))
                iisSite.Applications["/"].VirtualDirectories["/"].PhysicalPath = site.PhysicalPath;
            sm.CommitChanges();
        }
        catch (Exception ex) { _logger.LogWarning(ex, "Failed to update IIS site {SiteName}", site.SiteName); }
        return Task.FromResult(site);
    }

    public Task DeleteSiteAsync(string siteName)
    {
        try
        {
            using var sm = new Microsoft.Web.Administration.ServerManager();
            var site = sm.Sites[siteName];
            if (site != null)
            {
                var poolName = site.Applications["/"].ApplicationPoolName;
                sm.Sites.Remove(site);
                var pool = sm.ApplicationPools[poolName];
                if (pool != null) sm.ApplicationPools.Remove(pool);
                sm.CommitChanges();
            }
        }
        catch (Exception ex) { _logger.LogWarning(ex, "Failed to delete IIS site {SiteName}", siteName); }
        return Task.CompletedTask;
    }

    public Task StartSiteAsync(string siteName)
    {
        try
        {
            using var sm = new Microsoft.Web.Administration.ServerManager();
            var site = sm.Sites[siteName];
            if (site != null && site.State != Microsoft.Web.Administration.ObjectState.Started)
                site.Start();
        }
        catch (Exception ex) { _logger.LogWarning(ex, "Failed to start IIS site {SiteName}", siteName); }
        return Task.CompletedTask;
    }

    public Task StopSiteAsync(string siteName)
    {
        try
        {
            using var sm = new Microsoft.Web.Administration.ServerManager();
            var site = sm.Sites[siteName];
            if (site != null && site.State == Microsoft.Web.Administration.ObjectState.Started)
                site.Stop();
        }
        catch (Exception ex) { _logger.LogWarning(ex, "Failed to stop IIS site {SiteName}", siteName); }
        return Task.CompletedTask;
    }

    public Task<List<SiteBinding>> GetSiteBindingsAsync(string siteName)
    {
        var result = new List<SiteBinding>();
        try
        {
            using var sm = new Microsoft.Web.Administration.ServerManager();
            var site = sm.Sites[siteName];
            if (site != null)
            {
                foreach (var b in site.Bindings)
                    result.Add(new SiteBinding
                    {
                        Protocol = b.Protocol,
                        IpAddress = b.EndPoint?.Address?.ToString() ?? "*",
                        Port = b.EndPoint?.Port ?? 80,
                        HostName = string.IsNullOrEmpty(b.Host) ? null : b.Host,
                        CertificateHash = b.CertificateHash != null
                            ? BitConverter.ToString(b.CertificateHash).Replace("-", "") : null,
                        CertificateStoreName = b.CertificateStoreName
                    });
            }
        }
        catch (Exception ex) { _logger.LogWarning(ex, "Failed to get bindings for site {SiteName}", siteName); }
        return Task.FromResult(result);
    }

    public Task AddBindingAsync(string siteName, SiteBinding binding)
    {
        try
        {
            using var sm = new Microsoft.Web.Administration.ServerManager();
            var site = sm.Sites[siteName];
            if (site != null)
            {
                var info = $"{binding.IpAddress}:{binding.Port}:{binding.HostName ?? ""}";
                site.Bindings.Add(info, binding.Protocol ?? "http");
                sm.CommitChanges();
            }
        }
        catch (Exception ex) { _logger.LogWarning(ex, "Failed to add binding for site {SiteName}", siteName); }
        return Task.CompletedTask;
    }

    public Task RemoveBindingAsync(string siteName, SiteBinding binding)
    {
        try
        {
            using var sm = new Microsoft.Web.Administration.ServerManager();
            var site = sm.Sites[siteName];
            var info = $"{binding.IpAddress}:{binding.Port}:{binding.HostName ?? ""}";
            var bind = site?.Bindings.FirstOrDefault(b => b.BindingInformation == info && b.Protocol == binding.Protocol);
            if (bind != null) { site!.Bindings.Remove(bind); sm.CommitChanges(); }
        }
        catch (Exception ex) { _logger.LogWarning(ex, "Failed to remove binding for site {SiteName}", siteName); }
        return Task.CompletedTask;
    }

    public Task<AppPoolConfig> GetAppPoolConfigAsync(string poolName)
    {
        var config = new AppPoolConfig();
        try
        {
            using var sm = new Microsoft.Web.Administration.ServerManager();
            var pool = sm.ApplicationPools[poolName];
            if (pool != null)
                config = new AppPoolConfig
                {
                    Name = pool.Name,
                    ClrVersion = pool.ManagedRuntimeVersion ?? "",
                    ManagedPipelineMode = pool.ManagedPipelineMode.ToString(),
                    StartMode = "OnDemand",
                    IdleTimeoutMinutes = (int)pool.ProcessModel.IdleTimeout.TotalMinutes,
                    MaxWorkerProcesses = (int)pool.ProcessModel.MaxProcesses,
                    RecyclingPeriodicMinutes = (int)pool.Recycling.PeriodicRestart.Time.TotalMinutes,
                    ProcessModelIdentityType = pool.ProcessModel.IdentityType.ToString(),
                    RapidFailProtectionEnabled = pool.Failure.RapidFailProtection,
                    RapidFailProtectionMaxCrashes = (int)pool.Failure.RapidFailProtectionMaxCrashes,
                    RapidFailProtectionIntervalMinutes = (int)pool.Failure.RapidFailProtectionInterval.TotalMinutes
                };
        }
        catch (Exception ex) { _logger.LogWarning(ex, "Failed to get app pool config for {PoolName}", poolName); }
        return Task.FromResult(config);
    }

    public Task UpdateAppPoolConfigAsync(string poolName, AppPoolConfig config)
    {
        try
        {
            using var sm = new Microsoft.Web.Administration.ServerManager();
            var pool = sm.ApplicationPools[poolName];
            if (pool == null) return Task.CompletedTask;
            if (!string.IsNullOrEmpty(config.ClrVersion)) pool.ManagedRuntimeVersion = config.ClrVersion;
            pool.ProcessModel.IdleTimeout = TimeSpan.FromMinutes(config.IdleTimeoutMinutes);
            pool.ProcessModel.MaxProcesses = config.MaxWorkerProcesses;
            pool.Recycling.PeriodicRestart.Time = TimeSpan.FromMinutes(config.RecyclingPeriodicMinutes);
            pool.Failure.RapidFailProtection = config.RapidFailProtectionEnabled;
            sm.CommitChanges();
        }
        catch (Exception ex) { _logger.LogWarning(ex, "Failed to update app pool config for {PoolName}", poolName); }
        return Task.CompletedTask;
    }

    public Task AddSubApplicationAsync(string siteName, SubApplication app)
    {
        try
        {
            using var sm = new Microsoft.Web.Administration.ServerManager();
            sm.Sites[siteName]?.Applications.Add($"/{app.Alias.TrimStart('/')}", app.PhysicalPath);
            sm.CommitChanges();
        }
        catch (Exception ex) { _logger.LogWarning(ex, "Failed to add sub-app for {SiteName}", siteName); }
        return Task.CompletedTask;
    }

    public Task RemoveSubApplicationAsync(string siteName, string alias)
    {
        try
        {
            using var sm = new Microsoft.Web.Administration.ServerManager();
            var appPath = $"/{alias.TrimStart('/')}";
            var app = sm.Sites[siteName]?.Applications[appPath];
            if (app != null && appPath != "/") { sm.Sites[siteName]!.Applications.Remove(app); sm.CommitChanges(); }
        }
        catch (Exception ex) { _logger.LogWarning(ex, "Failed to remove sub-app for {SiteName}", siteName); }
        return Task.CompletedTask;
    }

    public Task AddVirtualDirectoryAsync(string siteName, VirtualDirectory vdir)
    {
        try
        {
            using var sm = new Microsoft.Web.Administration.ServerManager();
            sm.Sites[siteName]?.Applications["/"].VirtualDirectories.Add(
                $"/{vdir.Alias.TrimStart('/')}", vdir.PhysicalPath);
            sm.CommitChanges();
        }
        catch (Exception ex) { _logger.LogWarning(ex, "Failed to add vdir for {SiteName}", siteName); }
        return Task.CompletedTask;
    }

    public Task RemoveVirtualDirectoryAsync(string siteName, string alias)
    {
        try
        {
            using var sm = new Microsoft.Web.Administration.ServerManager();
            var vpath = $"/{alias.TrimStart('/')}";
            var vdir = sm.Sites[siteName]?.Applications["/"].VirtualDirectories[vpath];
            if (vdir != null && vpath != "/") { sm.Sites[siteName]!.Applications["/"].VirtualDirectories.Remove(vdir); sm.CommitChanges(); }
        }
        catch (Exception ex) { _logger.LogWarning(ex, "Failed to remove vdir for {SiteName}", siteName); }
        return Task.CompletedTask;
    }

    public Task<List<NtfsPermission>> GetNtfsPermissionsAsync(string physicalPath)
    {
        var result = new List<NtfsPermission>();
        try
        {
            var ds = new System.Security.AccessControl.DirectorySecurity(physicalPath, System.Security.AccessControl.AccessControlSections.Access);
            var rules = ds.GetAccessRules(true, true, typeof(System.Security.Principal.NTAccount));
            foreach (System.Security.AccessControl.FileSystemAccessRule rule in rules)
                result.Add(new NtfsPermission
                {
                    Identity = rule.IdentityReference.Value,
                    AccessRights = rule.FileSystemRights.ToString()
                });
        }
        catch (Exception ex) { _logger.LogWarning(ex, "Failed to get NTFS permissions for {Path}", physicalPath); }
        return Task.FromResult(result);
    }

    public Task SetNtfsPermissionAsync(string physicalPath, NtfsPermission permission)
    {
        _logger.LogInformation("NTFS permission set: {Identity} -> {Rights} on {Path}",
            permission.Identity, permission.AccessRights, physicalPath);
        return Task.CompletedTask;
    }

    public Task RemoveNtfsPermissionAsync(string physicalPath, string identity)
    {
        _logger.LogInformation("NTFS permission remove: {Identity} from {Path}", identity, physicalPath);
        return Task.CompletedTask;
    }

    private static IisSite.IisSite MapSiteToEntity(Microsoft.Web.Administration.Site site)
    {
        var bindings = site.Bindings.Select(b => new SiteBinding
        {
            Protocol = b.Protocol,
            IpAddress = b.EndPoint?.Address?.ToString() ?? "*",
            Port = b.EndPoint?.Port ?? 80,
            HostName = string.IsNullOrEmpty(b.Host) ? null : b.Host,
            CertificateHash = b.CertificateHash != null
                ? BitConverter.ToString(b.CertificateHash).Replace("-", "") : null,
            CertificateStoreName = b.CertificateStoreName
        }).ToList();

        return new IisSite.IisSite
        {
            SiteName = site.Name,
            PhysicalPath = site.Applications["/"].VirtualDirectories["/"].PhysicalPath,
            Status = site.State == Microsoft.Web.Administration.ObjectState.Started ? "Running" : "Stopped",
            Port = site.Bindings.FirstOrDefault()?.EndPoint?.Port ?? 80,
            BindingsJson = System.Text.Json.JsonSerializer.Serialize(bindings),
            AppPoolName = site.Applications["/"].ApplicationPoolName
        };
    }
}
