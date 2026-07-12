using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;

namespace AppManager.WindowsService;

public interface IWindowsServiceManager : ITransientDependency
{
    Task<List<WindowsService>> GetAllServicesAsync();
    Task<WindowsService?> GetServiceByNameAsync(string serviceName);
    Task<WindowsService> CreateServiceAsync(WindowsService service);
    Task<WindowsService> UpdateServiceAsync(WindowsService service);
    Task DeleteServiceAsync(string serviceName);
    Task StartServiceAsync(string serviceName);
    Task StopServiceAsync(string serviceName);
    Task RestartServiceAsync(string serviceName);
    Task<List<string>> GetServiceDependenciesAsync(string serviceName);
}