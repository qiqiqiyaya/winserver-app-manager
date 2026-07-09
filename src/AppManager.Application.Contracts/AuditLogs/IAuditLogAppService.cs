using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace AppManager.AuditLogs;

public interface IAuditLogAppService : IApplicationService
{
    Task<PagedResultDto<AuditLogDto>> GetListAsync(AuditLogFilterDto input);
    Task<AuditLogDetailDto> GetDetailAsync(Guid id);
}
