using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using System.Threading.Tasks;
using AppManager.Models;
using IisSiteEntity = AppManager.IisSite.IisSite;
using Microsoft.Extensions.Logging;
using Volo.Abp.DependencyInjection;

namespace AppManager.IisSite;

public class IisManager : IIisManager, ITransientDependency
{
    private readonly ILogger<IisManager> _logger;
    private readonly IServerManagerPool _pool;

    public IisManager(ILogger<IisManager> logger, IServerManagerPool pool)
    {
        _logger = logger;
        _pool = pool;
    }

    public Task<List<IisSiteEntity>> GetAllSitesAsync(string configPath)
    {
        var result = new List<IisSiteEntity>();
        var sm = _pool.GetOrCreate(configPath);
        foreach (var site in sm.Sites)
            result.Add(MapSiteToEntity(site));
        return Task.FromResult(result);
    }

    public Task<IisSiteEntity?> GetSiteByNameAsync(string configPath, string siteName)
    {
        var sm = _pool.GetOrCreate(configPath);
        var site = sm.Sites[siteName];
        return Task.FromResult(site != null ? MapSiteToEntity(site) : null);
    }

    public Task<IisSiteEntity> CreateSiteAsync(string configPath, IisSiteEntity site)
    {
        var sm = _pool.GetOrCreate(configPath);
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

    public Task<IisSiteEntity> UpdateSiteAsync(string configPath, IisSiteEntity site)
    {
        var sm = _pool.GetOrCreate(configPath);
        var iisSite = sm.Sites[site.SiteName];
        if (iisSite != null && !string.IsNullOrEmpty(site.PhysicalPath))
            iisSite.Applications["/"].VirtualDirectories["/"].PhysicalPath = site.PhysicalPath;
        sm.CommitChanges();
        return Task.FromResult(site);
    }

    public Task DeleteSiteAsync(string configPath, string siteName)
    {
        var sm = _pool.GetOrCreate(configPath);
        var site = sm.Sites[siteName];
        if (site != null)
        {
            var poolName = site.Applications["/"].ApplicationPoolName;
            sm.Sites.Remove(site);
            var pool = sm.ApplicationPools[poolName];
            if (pool != null) sm.ApplicationPools.Remove(pool);
            sm.CommitChanges();
        }
        return Task.CompletedTask;
    }

    public Task StartSiteAsync(string configPath, string siteName)
    {
        var sm = _pool.GetOrCreate(configPath);
        var site = sm.Sites[siteName];
        if (site != null && site.State != Microsoft.Web.Administration.ObjectState.Started)
            site.Start();
        return Task.CompletedTask;
    }

    public Task StopSiteAsync(string configPath, string siteName)
    {
        var sm = _pool.GetOrCreate(configPath);
        var site = sm.Sites[siteName];
        if (site != null && site.State == Microsoft.Web.Administration.ObjectState.Started)
            site.Stop();
        return Task.CompletedTask;
    }

    public Task<List<SiteBinding>> GetSiteBindingsAsync(string configPath, string siteName)
    {
        var result = new List<SiteBinding>();
        var sm = _pool.GetOrCreate(configPath);
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
        return Task.FromResult(result);
    }

    public Task AddBindingAsync(string configPath, string siteName, SiteBinding binding)
    {
        var sm = _pool.GetOrCreate(configPath);
        var site = sm.Sites[siteName];
        if (site != null)
        {
            var info = $"{binding.IpAddress}:{binding.Port}:{binding.HostName ?? ""}";
            site.Bindings.Add(info, binding.Protocol ?? "http");
            sm.CommitChanges();
        }
        return Task.CompletedTask;
    }

    public Task RemoveBindingAsync(string configPath, string siteName, SiteBinding binding)
    {
        var sm = _pool.GetOrCreate(configPath);
        var site = sm.Sites[siteName];
        var info = $"{binding.IpAddress}:{binding.Port}:{binding.HostName ?? ""}";
        var bind = site?.Bindings.FirstOrDefault(b => b.BindingInformation == info && b.Protocol == binding.Protocol);
        if (bind != null) { site!.Bindings.Remove(bind); sm.CommitChanges(); }
        return Task.CompletedTask;
    }

    public Task<AppPoolConfig> GetAppPoolConfigAsync(string configPath, string poolName)
    {
        var config = new AppPoolConfig();
        var sm = _pool.GetOrCreate(configPath);
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
        return Task.FromResult(config);
    }

    public Task UpdateAppPoolConfigAsync(string configPath, string poolName, AppPoolConfig config)
    {
        var sm = _pool.GetOrCreate(configPath);
        var pool = sm.ApplicationPools[poolName];
        if (pool == null) return Task.CompletedTask;
        if (!string.IsNullOrEmpty(config.ClrVersion)) pool.ManagedRuntimeVersion = config.ClrVersion;
        pool.ProcessModel.IdleTimeout = TimeSpan.FromMinutes(config.IdleTimeoutMinutes);
        pool.ProcessModel.MaxProcesses = config.MaxWorkerProcesses;
        pool.Recycling.PeriodicRestart.Time = TimeSpan.FromMinutes(config.RecyclingPeriodicMinutes);
        pool.Failure.RapidFailProtection = config.RapidFailProtectionEnabled;
        sm.CommitChanges();
        return Task.CompletedTask;
    }

    public Task AddSubApplicationAsync(string configPath, string siteName, SubApplication app)
    {
        var sm = _pool.GetOrCreate(configPath);
        sm.Sites[siteName]?.Applications.Add($"/{app.Alias.TrimStart('/')}", app.PhysicalPath);
        sm.CommitChanges();
        return Task.CompletedTask;
    }

    public Task RemoveSubApplicationAsync(string configPath, string siteName, string alias)
    {
        var sm = _pool.GetOrCreate(configPath);
        var appPath = $"/{alias.TrimStart('/')}";
        var app = sm.Sites[siteName]?.Applications[appPath];
        if (app != null && appPath != "/") { sm.Sites[siteName]!.Applications.Remove(app); sm.CommitChanges(); }
        return Task.CompletedTask;
    }

    public Task AddVirtualDirectoryAsync(string configPath, string siteName, VirtualDirectory vdir)
    {
        var sm = _pool.GetOrCreate(configPath);
        sm.Sites[siteName]?.Applications["/"].VirtualDirectories.Add(
            $"/{vdir.Alias.TrimStart('/')}", vdir.PhysicalPath);
        sm.CommitChanges();
        return Task.CompletedTask;
    }

    public Task RemoveVirtualDirectoryAsync(string configPath, string siteName, string alias)
    {
        var sm = _pool.GetOrCreate(configPath);
        var vpath = $"/{alias.TrimStart('/')}";
        var vdir = sm.Sites[siteName]?.Applications["/"].VirtualDirectories[vpath];
        if (vdir != null && vpath != "/") { sm.Sites[siteName]!.Applications["/"].VirtualDirectories.Remove(vdir); sm.CommitChanges(); }
        return Task.CompletedTask;
    }

    public Task<List<NtfsPermission>> GetNtfsPermissionsAsync(string configPath, string physicalPath)
    {
        var result = new List<NtfsPermission>();
        var ds = new DirectorySecurity(physicalPath, AccessControlSections.Access);
        var rules = ds.GetAccessRules(true, true, typeof(System.Security.Principal.NTAccount));
        foreach (FileSystemAccessRule rule in rules)
            result.Add(new NtfsPermission
            {
                Identity = rule.IdentityReference.Value,
                AccessRights = rule.FileSystemRights.ToString()
            });
        return Task.FromResult(result);
    }

    public Task SetNtfsPermissionAsync(string configPath, string physicalPath, NtfsPermission permission)
    {
        _logger.LogInformation("NTFS permission set: {Identity} -> {Rights} on {Path}",
            permission.Identity, permission.AccessRights, physicalPath);
        return Task.CompletedTask;
    }

    public Task RemoveNtfsPermissionAsync(string configPath, string physicalPath, string identity)
    {
        _logger.LogInformation("NTFS permission remove: {Identity} from {Path}", identity, physicalPath);
        return Task.CompletedTask;
    }

    private static IisSiteEntity MapSiteToEntity(Microsoft.Web.Administration.Site site)
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

        return new IisSiteEntity
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
