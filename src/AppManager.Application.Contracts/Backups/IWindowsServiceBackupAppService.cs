using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace AppManager.Backups;

public interface IWindowsServiceBackupAppService : IApplicationService
{
    Task<PagedResultDto<WindowsServiceBackupDto>> GetListAsync(GetBackupListDto input);
    Task<WindowsServiceBackupDto> CreateAsync(CreateBackupDto input);
    Task<string> PreviewAsync(Guid id);
    Task RestoreAsync(RestoreBackupDto input);
    Task DeleteAsync(Guid id);
}
