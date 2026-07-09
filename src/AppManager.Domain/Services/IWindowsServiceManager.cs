using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;

namespace AppManager.Services;

public interface IWindowsServiceManager : ITransientDependency
{
    Task<List<WindowsService.WindowsService>> GetAllServicesAsync();
    Task<WindowsService.WindowsService?> GetServiceByNameAsync(string serviceName);
    Task<WindowsService.WindowsService> CreateServiceAsync(WindowsService.WindowsService service);
    Task<WindowsService.WindowsService> UpdateServiceAsync(WindowsService.WindowsService service);
    Task DeleteServiceAsync(string serviceName);
    Task StartServiceAsync(string serviceName);
    Task StopServiceAsync(string serviceName);
    Task RestartServiceAsync(string serviceName);
    Task<List<string>> GetServiceDependenciesAsync(string serviceName);
}
