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

[Authorize(AppManagerPermissions.IisSites.Backup)]
public class IisSiteBackupAppService : AppManagerAppService, IIisSiteBackupAppService
{
    private readonly IIisBackupService _backupService;
    private readonly IRepository<IisSiteBackup, Guid> _backupRepository;

    public IisSiteBackupAppService(
        IIisBackupService backupService,
        IRepository<IisSiteBackup, Guid> backupRepository)
    {
        _backupService = backupService;
        _backupRepository = backupRepository;
    }

    public async Task<PagedResultDto<IisSiteBackupDto>> GetListAsync(GetBackupListDto input)
    {
        var query = string.IsNullOrWhiteSpace(input.Filter)
            ? await _backupRepository.GetQueryableAsync()
            : (await _backupRepository.GetQueryableAsync())
                .Where(b => b.SiteName.Contains(input.Filter));
        var totalCount = query.Count();
        var items = query.OrderByDescending(b => b.CreatedAt)
            .Skip(input.SkipCount).Take(input.MaxResultCount).ToList();
        return new PagedResultDto<IisSiteBackupDto>(
            totalCount,
            ObjectMapper.Map<List<IisSiteBackup>, List<IisSiteBackupDto>>(items));
    }

    [Authorize(AppManagerPermissions.IisSites.Backup)]
    public async Task<IisSiteBackupDto> CreateAsync(CreateBackupDto input)
    {
        var backup = await _backupService.CreateBackupAsync(input.SiteOrServiceName, input.Description);
        var entity = await _backupRepository.InsertAsync(backup, autoSave: true);
        return ObjectMapper.Map<IisSiteBackup, IisSiteBackupDto>(entity);
    }

    public async Task<string> PreviewAsync(Guid id)
    {
        return await _backupService.PreviewBackupAsync(id);
    }

    [Authorize(AppManagerPermissions.IisSites.Restore)]
    public async Task RestoreAsync(RestoreBackupDto input)
    {
        await _backupService.RestoreFromBackupAsync(input.BackupId, input.Overwrite, input.NewSiteName);
    }

    public async Task DeleteAsync(Guid id)
    {
        await _backupRepository.DeleteAsync(id, autoSave: true);
    }
}
