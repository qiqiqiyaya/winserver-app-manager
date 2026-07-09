using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace AppManager.WindowsServices;

public interface IWindowsServiceAppService : IApplicationService
{
    Task<PagedResultDto<WindowsServiceDto>> GetListAsync(GetWindowsServiceListDto input);
    Task<WindowsServiceDto> GetAsync(Guid id);
    Task<WindowsServiceDto> CreateAsync(CreateWindowsServiceDto input);
    Task<WindowsServiceDto> UpdateAsync(Guid id, UpdateWindowsServiceDto input);
    Task DeleteAsync(Guid id);
    Task StartAsync(Guid id);
    Task StopAsync(Guid id);
    Task RestartAsync(Guid id);
}
