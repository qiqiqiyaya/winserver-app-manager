using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AppManager.IisSite;
using IisSiteEntity = AppManager.IisSite.IisSite;
using AppManager.IisSites;
using AppManager.Models;
using AppManager.Permissions;
using AppManager.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Repositories;

namespace AppManager.Application.IisSites;

[Authorize(AppManagerPermissions.IisSites.Default)]
public class IisSiteAppService : AppManagerAppService, IIisSiteAppService
{
    private readonly IIisManager _iisManager;
    private readonly IRepository<IisSiteEntity, Guid> _siteRepository;

    public IisSiteAppService(
        IIisManager iisManager,
        IRepository<IisSiteEntity, Guid> siteRepository)
    {
        _iisManager = iisManager;
        _siteRepository = siteRepository;
    }

    public async Task<PagedResultDto<IisSiteDto>> GetListAsync(GetIisSiteListDto input)
    {
        var query = string.IsNullOrWhiteSpace(input.Filter)
            ? await _siteRepository.GetQueryableAsync()
            : (await _siteRepository.GetQueryableAsync())
                .Where(s => s.SiteName.Contains(input.Filter));

        var totalCount = query.Count();
        var items = query.OrderBy(s => s.SiteName)
            .Skip(input.SkipCount).Take(input.MaxResultCount).ToList();

        return new PagedResultDto<IisSiteDto>(
            totalCount,
            ObjectMapper.Map<List<IisSiteEntity>, List<IisSiteDto>>(items));
    }

    public async Task<IisSiteDto> GetAsync(Guid id)
    {
        var entity = await _siteRepository.GetAsync(id);
        return ObjectMapper.Map<IisSiteEntity, IisSiteDto>(entity);
    }

    [Authorize(AppManagerPermissions.IisSites.Create)]
    public async Task<IisSiteDto> CreateAsync(CreateIisSiteDto input)
    {
        var existing = await _siteRepository.FindAsync(s => s.SiteName == input.SiteName);
        if (existing != null)
            throw new BusinessException(AppManagerDomainErrorCodes.SiteAlreadyExists);

        var entity = ObjectMapper.Map<CreateIisSiteDto, IisSiteEntity>(input);
        entity.Status = "Unknown";

        try { entity = await _iisManager.CreateSiteAsync(entity); entity.Status = "Running"; }
        catch (Exception ex) { Logger.LogWarning(ex, "Failed to create IIS site {SiteName}", input.SiteName); }

        entity = await _siteRepository.InsertAsync(entity, autoSave: true);
        return ObjectMapper.Map<IisSiteEntity, IisSiteDto>(entity);
    }

    [Authorize(AppManagerPermissions.IisSites.Edit)]
    public async Task<IisSiteDto> UpdateAsync(Guid id, UpdateIisSiteDto input)
    {
        var entity = await _siteRepository.GetAsync(id);
        if (input.SiteName != null) entity.SiteName = input.SiteName;
        if (input.PhysicalPath != null) entity.PhysicalPath = input.PhysicalPath;
        if (input.Port.HasValue) entity.Port = input.Port.Value;
        try { entity = await _iisManager.UpdateSiteAsync(entity); }
        catch (Exception ex) { Logger.LogWarning(ex, "Failed to update site {SiteName}", entity.SiteName); }
        await _siteRepository.UpdateAsync(entity, autoSave: true);
        return ObjectMapper.Map<IisSiteEntity, IisSiteDto>(entity);
    }

    [Authorize(AppManagerPermissions.IisSites.Delete)]
    public async Task DeleteAsync(Guid id)
    {
        var entity = await _siteRepository.GetAsync(id);
        try { await _iisManager.DeleteSiteAsync(entity.SiteName); }
        catch (Exception ex) { Logger.LogWarning(ex, "Failed to delete site {SiteName}", entity.SiteName); }
        await _siteRepository.DeleteAsync(entity, autoSave: true);
    }

    public async Task StartAsync(Guid id)
    {
        var entity = await _siteRepository.GetAsync(id);
        await _iisManager.StartSiteAsync(entity.SiteName);
        entity.Status = "Running";
        await _siteRepository.UpdateAsync(entity, autoSave: true);
    }

    public async Task StopAsync(Guid id)
    {
        var entity = await _siteRepository.GetAsync(id);
        await _iisManager.StopSiteAsync(entity.SiteName);
        entity.Status = "Stopped";
        await _siteRepository.UpdateAsync(entity, autoSave: true);
    }

    [Authorize(AppManagerPermissions.IisSites.ManageBinding)]
    public async Task<List<SiteBindingDto>> GetBindingsAsync(Guid id)
    {
        var entity = await _siteRepository.GetAsync(id);
        var bindings = await _iisManager.GetSiteBindingsAsync(entity.SiteName);
        return ObjectMapper.Map<List<SiteBinding>, List<SiteBindingDto>>(bindings);
    }

    [Authorize(AppManagerPermissions.IisSites.ManageBinding)]
    public async Task AddBindingAsync(Guid id, SiteBindingDto input)
    {
        var entity = await _siteRepository.GetAsync(id);
        var binding = ObjectMapper.Map<SiteBindingDto, SiteBinding>(input);
        await _iisManager.AddBindingAsync(entity.SiteName, binding);
        var bindings = await _iisManager.GetSiteBindingsAsync(entity.SiteName);
        entity.BindingsJson = System.Text.Json.JsonSerializer.Serialize(bindings);
        await _siteRepository.UpdateAsync(entity, autoSave: true);
    }

    [Authorize(AppManagerPermissions.IisSites.ManageBinding)]
    public async Task RemoveBindingAsync(Guid id, SiteBindingDto input)
    {
        var entity = await _siteRepository.GetAsync(id);
        var binding = ObjectMapper.Map<SiteBindingDto, SiteBinding>(input);
        await _iisManager.RemoveBindingAsync(entity.SiteName, binding);
        var bindings = await _iisManager.GetSiteBindingsAsync(entity.SiteName);
        entity.BindingsJson = System.Text.Json.JsonSerializer.Serialize(bindings);
        await _siteRepository.UpdateAsync(entity, autoSave: true);
    }

    [Authorize(AppManagerPermissions.IisSites.ManageAppPool)]
    public async Task<AppPoolConfigDto> GetAppPoolConfigAsync(Guid id)
    {
        var entity = await _siteRepository.GetAsync(id);
        if (entity.AppPoolName == null) return new AppPoolConfigDto();
        var config = await _iisManager.GetAppPoolConfigAsync(entity.AppPoolName);
        return ObjectMapper.Map<AppPoolConfig, AppPoolConfigDto>(config);
    }

    [Authorize(AppManagerPermissions.IisSites.ManageAppPool)]
    public async Task UpdateAppPoolConfigAsync(Guid id, AppPoolConfigDto input)
    {
        var entity = await _siteRepository.GetAsync(id);
        if (entity.AppPoolName == null) return;
        var config = ObjectMapper.Map<AppPoolConfigDto, AppPoolConfig>(input);
        await _iisManager.UpdateAppPoolConfigAsync(entity.AppPoolName, config);
        entity.AppPoolConfigJson = System.Text.Json.JsonSerializer.Serialize(input);
        await _siteRepository.UpdateAsync(entity, autoSave: true);
    }

    public async Task<List<SubApplicationDto>> GetSubApplicationsAsync(Guid id)
    {
        var entity = await _siteRepository.GetAsync(id);
        if (entity.SubApplicationsJson == null) return new List<SubApplicationDto>();
        return System.Text.Json.JsonSerializer.Deserialize<List<SubApplicationDto>>(entity.SubApplicationsJson)
               ?? new List<SubApplicationDto>();
    }

    public async Task AddSubApplicationAsync(Guid id, SubApplicationDto input)
    {
        var entity = await _siteRepository.GetAsync(id);
        await _iisManager.AddSubApplicationAsync(entity.SiteName,
            ObjectMapper.Map<SubApplicationDto, SubApplication>(input));
        var apps = await GetSubApplicationsAsync(id);
        apps.Add(input);
        entity.SubApplicationsJson = System.Text.Json.JsonSerializer.Serialize(apps);
        await _siteRepository.UpdateAsync(entity, autoSave: true);
    }

    public async Task RemoveSubApplicationAsync(Guid id, string alias)
    {
        var entity = await _siteRepository.GetAsync(id);
        await _iisManager.RemoveSubApplicationAsync(entity.SiteName, alias);
        var apps = await GetSubApplicationsAsync(id);
        apps.RemoveAll(a => a.Alias == alias);
        entity.SubApplicationsJson = System.Text.Json.JsonSerializer.Serialize(apps);
        await _siteRepository.UpdateAsync(entity, autoSave: true);
    }

    public async Task<List<VirtualDirectoryDto>> GetVirtualDirectoriesAsync(Guid id)
    {
        var entity = await _siteRepository.GetAsync(id);
        if (entity.VirtualDirectoriesJson == null) return new List<VirtualDirectoryDto>();
        return System.Text.Json.JsonSerializer.Deserialize<List<VirtualDirectoryDto>>(entity.VirtualDirectoriesJson)
               ?? new List<VirtualDirectoryDto>();
    }

    public async Task AddVirtualDirectoryAsync(Guid id, VirtualDirectoryDto input)
    {
        var entity = await _siteRepository.GetAsync(id);
        await _iisManager.AddVirtualDirectoryAsync(entity.SiteName,
            ObjectMapper.Map<VirtualDirectoryDto, VirtualDirectory>(input));
        var vdirs = await GetVirtualDirectoriesAsync(id);
        vdirs.Add(input);
        entity.VirtualDirectoriesJson = System.Text.Json.JsonSerializer.Serialize(vdirs);
        await _siteRepository.UpdateAsync(entity, autoSave: true);
    }

    public async Task RemoveVirtualDirectoryAsync(Guid id, string alias)
    {
        var entity = await _siteRepository.GetAsync(id);
        await _iisManager.RemoveVirtualDirectoryAsync(entity.SiteName, alias);
        var vdirs = await GetVirtualDirectoriesAsync(id);
        vdirs.RemoveAll(v => v.Alias == alias);
        entity.VirtualDirectoriesJson = System.Text.Json.JsonSerializer.Serialize(vdirs);
        await _siteRepository.UpdateAsync(entity, autoSave: true);
    }

    [Authorize(AppManagerPermissions.IisSites.ManagePermissions)]
    public async Task<List<NtfsPermissionDto>> GetNtfsPermissionsAsync(Guid id)
    {
        var entity = await _siteRepository.GetAsync(id);
        return await _iisManager.GetNtfsPermissionsAsync(entity.PhysicalPath) is { } perms
            ? ObjectMapper.Map<List<NtfsPermission>, List<NtfsPermissionDto>>(perms)
            : new List<NtfsPermissionDto>();
    }

    [Authorize(AppManagerPermissions.IisSites.ManagePermissions)]
    public async Task SetNtfsPermissionAsync(Guid id, NtfsPermissionDto input)
    {
        var entity = await _siteRepository.GetAsync(id);
        var perm = ObjectMapper.Map<NtfsPermissionDto, NtfsPermission>(input);
        await _iisManager.SetNtfsPermissionAsync(entity.PhysicalPath, perm);
    }

    [Authorize(AppManagerPermissions.IisSites.ManagePermissions)]
    public async Task RemoveNtfsPermissionAsync(Guid id, string identity)
    {
        var entity = await _siteRepository.GetAsync(id);
        await _iisManager.RemoveNtfsPermissionAsync(entity.PhysicalPath, identity);
    }
}
