using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Volo.Abp.DependencyInjection;

namespace AppManager.WindowsService;

public class WindowsServiceManager : IWindowsServiceManager, ITransientDependency
{
    private readonly ILogger<WindowsServiceManager> _logger;

    public WindowsServiceManager(ILogger<WindowsServiceManager> logger)
    {
        _logger = logger;
    }

    public Task<List<WindowsService>> GetAllServicesAsync()
    {
        var result = new List<WindowsService>();
        foreach (var sc in ServiceController.GetServices())
            result.Add(MapToEntity(sc));
        return Task.FromResult(result);
    }

    public Task<WindowsService?> GetServiceByNameAsync(string serviceName)
    {
        using var sc = new ServiceController(serviceName);
        return Task.FromResult<WindowsService?>(MapToEntity(sc));
    }

    public Task<WindowsService> CreateServiceAsync(WindowsService service)
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
        return Task.FromResult(service);
    }

    public Task<WindowsService> UpdateServiceAsync(WindowsService service)
    {
        var start = service.StartType switch
        {
            "Automatic" => "auto", "DelayedAuto" => "delayed-auto",
            "Disabled" => "disabled", _ => "demand"
        };
        RunSc($"config \"{service.ServiceName}\" start= {start}");
        return Task.FromResult(service);
    }

    public Task DeleteServiceAsync(string serviceName)
    {
        RunSc($"delete \"{serviceName}\"");
        return Task.CompletedTask;
    }

    public Task StartServiceAsync(string serviceName)
    {
        using var sc = new ServiceController(serviceName);
        if (sc.Status != ServiceControllerStatus.Running)
        { sc.Start(); sc.WaitForStatus(ServiceControllerStatus.Running, TimeSpan.FromSeconds(30)); }
        return Task.CompletedTask;
    }

    public Task StopServiceAsync(string serviceName)
    {
        using var sc = new ServiceController(serviceName);
        if (sc.CanStop && sc.Status == ServiceControllerStatus.Running)
        { sc.Stop(); sc.WaitForStatus(ServiceControllerStatus.Stopped, TimeSpan.FromSeconds(30)); }
        return Task.CompletedTask;
    }

    public Task RestartServiceAsync(string serviceName)
    {
        using var sc = new ServiceController(serviceName);
        if (sc.Status == ServiceControllerStatus.Running)
        { sc.Stop(); sc.WaitForStatus(ServiceControllerStatus.Stopped, TimeSpan.FromSeconds(30)); }
        sc.Start(); sc.WaitForStatus(ServiceControllerStatus.Running, TimeSpan.FromSeconds(30));
        return Task.CompletedTask;
    }

    public Task<List<string>> GetServiceDependenciesAsync(string serviceName)
    {
        using var sc = new ServiceController(serviceName);
        return Task.FromResult(sc.ServicesDependedOn.Select(d => d.ServiceName).ToList());
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

    private static WindowsService MapToEntity(ServiceController sc) => new()
    {
        ServiceName = sc.ServiceName,
        DisplayName = sc.DisplayName,
        Status = sc.Status.ToString(),
        StartType = sc.StartType.ToString(),
        ExecutablePath = ""
    };
}