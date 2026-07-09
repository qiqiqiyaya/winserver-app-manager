using System;
using System.Linq;
using System.Threading.Tasks;
using AppManager.AuditLogs;
using AppManager.Permissions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using Volo.Abp.Application.Dtos;

namespace AppManager.Application.AuditLogs;

[Authorize(AppManagerPermissions.AuditLogs.View)]
public class AuditLogAppService : AppManagerAppService, IAuditLogAppService
{
    private readonly ILogger<AuditLogAppService> _logger;

    public AuditLogAppService(ILogger<AuditLogAppService> logger)
    {
        _logger = logger;
    }

    public async Task<PagedResultDto<AuditLogDto>> GetListAsync(AuditLogFilterDto input)
    {
        _logger.LogInformation("Audit log query: User={User}, Method={Method}", input.UserName, input.HttpMethod);
        return new PagedResultDto<AuditLogDto>(0, Enumerable.Empty<AuditLogDto>().ToList());
    }

    public async Task<AuditLogDetailDto> GetDetailAsync(Guid id)
    {
        throw new NotImplementedException("Audit log detail not yet implemented.");
    }
}
