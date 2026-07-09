using System;
using System.Linq;
using System.Threading.Tasks;
using AppManager.Permissions;
using AppManager.SystemLogs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using Volo.Abp.Application.Dtos;

namespace AppManager.Application.SystemLogs;

[Authorize(AppManagerPermissions.SystemLogs.View)]
public class SystemLogAppService : AppManagerAppService, ISystemLogAppService
{
    private readonly ILogger<SystemLogAppService> _logger;

    public SystemLogAppService(ILogger<SystemLogAppService> logger)
    {
        _logger = logger;
    }

    public async Task<PagedResultDto<SystemLogDto>> GetListAsync(SystemLogFilterDto input)
    {
        _logger.LogInformation("System log query: Level={Level}, Keyword={Keyword}", input.Level, input.Keyword);
        return new PagedResultDto<SystemLogDto>(0, Enumerable.Empty<SystemLogDto>().ToList());
    }

    public async Task<SystemLogDto> GetAsync(Guid id)
    {
        throw new NotImplementedException("Log detail not implemented.");
    }

    public async Task ClearAsync()
    {
        _logger.LogWarning("Clear logs requested but not implemented.");
    }
}
