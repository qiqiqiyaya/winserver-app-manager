using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace AppManager.Backups;

public interface IIisSiteBackupAppService : IApplicationService
{
    Task<PagedResultDto<IisSiteBackupDto>> GetListAsync(GetBackupListDto input);
    Task<IisSiteBackupDto> CreateAsync(CreateBackupDto input);
    Task<string> PreviewAsync(Guid id);
    Task RestoreAsync(RestoreBackupDto input);
    Task DeleteAsync(Guid id);
}
