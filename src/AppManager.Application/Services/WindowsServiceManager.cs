using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Volo.Abp.DependencyInjection;

namespace AppManager.Services;

public class WindowsServiceManager : IWindowsServiceManager, ITransientDependency
{
    private readonly ILogger<WindowsServiceManager> _logger;

    public WindowsServiceManager(ILogger<WindowsServiceManager> logger)
    {
        _logger = logger;
    }

    public Task<List<WindowsService.WindowsService>> GetAllServicesAsync()
    {
        var result = new List<WindowsService.WindowsService>();
        try
        {
            foreach (var sc in ServiceController.GetServices())
                result.Add(MapToEntity(sc));
        }
        catch (Exception ex) { _logger.LogWarning(ex, "Failed to enumerate services"); }
        return Task.FromResult(result);
    }

    public Task<WindowsService.WindowsService?> GetServiceByNameAsync(string serviceName)
    {
        try
        {
            using var sc = new ServiceController(serviceName);
            return Task.FromResult<WindowsService.WindowsService?>(MapToEntity(sc));
        }
        catch { return Task.FromResult<WindowsService.WindowsService?>(null); }
    }

    public Task<WindowsService.WindowsService> CreateServiceAsync(WindowsService.WindowsService service)
    {
        try
        {
            var start = service.StartType switch
            {
                "Automatic" => "auto", "DelayedAuto" => "delayed-auto",
                "Disabled" => "disabled", _ => "demand"
            };
            var args = $"create \"{service.ServiceName}\" binPath= \"{service.ExecutablePath}\" start= {start}";
            if (!string.IsNullOrEmpty(service.DisplayName))
                args += $" DisplayName= \"{service.DisplayName}\"";

            RunSc(args);
            if (service.Account != "LocalSystem" && !string.IsNullOrEmpty(service.Password))
                RunSc($"config \"{service.ServiceName}\" obj= \"{service.Account}\" password= \"{service.Password}\"");

            service.Status = "Stopped";
        }
        catch (Exception ex) { _logger.LogWarning(ex, "Failed to create service {ServiceName}", service.ServiceName); }
        return Task.FromResult(service);
    }

    public Task<WindowsService.WindowsService> UpdateServiceAsync(WindowsService.WindowsService service)
    {
        try
        {
            var start = service.StartType switch
            {
                "Automatic" => "auto", "DelayedAuto" => "delayed-auto",
                "Disabled" => "disabled", _ => "demand"
            };
            RunSc($"config \"{service.ServiceName}\" start= {start}");
        }
        catch (Exception ex) { _logger.LogWarning(ex, "Failed to update service {ServiceName}", service.ServiceName); }
        return Task.FromResult(service);
    }

    public Task DeleteServiceAsync(string serviceName)
    {
        try { RunSc($"delete \"{serviceName}\""); }
        catch (Exception ex) { _logger.LogWarning(ex, "Failed to delete service {ServiceName}", serviceName); }
        return Task.CompletedTask;
    }

    public Task StartServiceAsync(string serviceName)
    {
        try
        {
            using var sc = new ServiceController(serviceName);
            if (sc.Status != ServiceControllerStatus.Running)
            { sc.Start(); sc.WaitForStatus(ServiceControllerStatus.Running, TimeSpan.FromSeconds(30)); }
        }
        catch (Exception ex) { _logger.LogWarning(ex, "Failed to start service {ServiceName}", serviceName); }
        return Task.CompletedTask;
    }

    public Task StopServiceAsync(string serviceName)
    {
        try
        {
            using var sc = new ServiceController(serviceName);
            if (sc.CanStop && sc.Status == ServiceControllerStatus.Running)
            { sc.Stop(); sc.WaitForStatus(ServiceControllerStatus.Stopped, TimeSpan.FromSeconds(30)); }
        }
        catch (Exception ex) { _logger.LogWarning(ex, "Failed to stop service {ServiceName}", serviceName); }
        return Task.CompletedTask;
    }

    public Task RestartServiceAsync(string serviceName)
    {
        try
        {
            using var sc = new ServiceController(serviceName);
            if (sc.Status == ServiceControllerStatus.Running)
            { sc.Stop(); sc.WaitForStatus(ServiceControllerStatus.Stopped, TimeSpan.FromSeconds(30)); }
            sc.Start(); sc.WaitForStatus(ServiceControllerStatus.Running, TimeSpan.FromSeconds(30));
        }
        catch (Exception ex) { _logger.LogWarning(ex, "Failed to restart service {ServiceName}", serviceName); }
        return Task.CompletedTask;
    }

    public Task<List<string>> GetServiceDependenciesAsync(string serviceName)
    {
        try
        {
            using var sc = new ServiceController(serviceName);
            return Task.FromResult(sc.ServicesDependedOn.Select(d => d.ServiceName).ToList());
        }
        catch { return Task.FromResult(new List<string>()); }
    }

    private static void RunSc(string args)
    {
        using var p = System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
        {
            FileName = "sc.exe", Arguments = args,
            RedirectStandardOutput = true, UseShellExecute = false, CreateNoWindow = true
        });
        p?.WaitForExit(5000);
    }

    private static WindowsService.WindowsService MapToEntity(ServiceController sc) => new()
    {
        ServiceName = sc.ServiceName,
        DisplayName = sc.DisplayName,
        Status = sc.Status.ToString(),
        StartType = sc.StartType.ToString(),
        ExecutablePath = ""
    };
}
