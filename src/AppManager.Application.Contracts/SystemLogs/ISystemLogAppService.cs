using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace AppManager.SystemLogs;

public interface ISystemLogAppService : IApplicationService
{
    Task<PagedResultDto<SystemLogDto>> GetListAsync(SystemLogFilterDto input);
    Task<SystemLogDto> GetAsync(Guid id);
    Task ClearAsync();
}
