using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AppManager.Backups;
using AppManager.Permissions;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Repositories;

namespace AppManager.Application.Backups;

[Authorize(AppManagerPermissions.WindowsServices.Backup)]
public class WindowsServiceBackupAppService : AppManagerAppService, IWindowsServiceBackupAppService
{
    private readonly IWindowsServiceBackupService _backupService;
    private readonly IRepository<WindowsServiceBackup, Guid> _backupRepository;

    public WindowsServiceBackupAppService(
        IWindowsServiceBackupService backupService,
        IRepository<WindowsServiceBackup, Guid> backupRepository)
    {
        _backupService = backupService;
        _backupRepository = backupRepository;
    }

    public async Task<PagedResultDto<WindowsServiceBackupDto>> GetListAsync(GetBackupListDto input)
    {
        var query = string.IsNullOrWhiteSpace(input.Filter)
            ? await _backupRepository.GetQueryableAsync()
            : (await _backupRepository.GetQueryableAsync())
                .Where(b => b.ServiceName.Contains(input.Filter));
        var totalCount = query.Count();
        var items = query.OrderByDescending(b => b.CreatedAt)
            .Skip(input.SkipCount).Take(input.MaxResultCount).ToList();
        return new PagedResultDto<WindowsServiceBackupDto>(
            totalCount,
            ObjectMapper.Map<List<WindowsServiceBackup>, List<WindowsServiceBackupDto>>(items));
    }

    [Authorize(AppManagerPermissions.WindowsServices.Backup)]
    public async Task<WindowsServiceBackupDto> CreateAsync(CreateBackupDto input)
    {
        var backup = await _backupService.CreateBackupAsync(input.SiteOrServiceName, input.Description);
        var entity = await _backupRepository.InsertAsync(backup, autoSave: true);
        return ObjectMapper.Map<WindowsServiceBackup, WindowsServiceBackupDto>(entity);
    }

    public async Task<string> PreviewAsync(Guid id)
    {
        return await _backupService.PreviewBackupAsync(id);
    }

    [Authorize(AppManagerPermissions.WindowsServices.Restore)]
    public async Task RestoreAsync(RestoreBackupDto input)
    {
        await _backupService.RestoreFromBackupAsync(input.BackupId, input.Overwrite);
    }

    public async Task DeleteAsync(Guid id)
    {
        await _backupRepository.DeleteAsync(id, autoSave: true);
    }
}
