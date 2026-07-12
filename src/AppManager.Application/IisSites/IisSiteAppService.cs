using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AppManager.IisSite;
using IisSiteEntity = AppManager.IisSite.IisSite;
using AppManager.IisSites;
using AppManager.Models;
using AppManager.Permissions;
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
    private readonly IRepository<IisInstance, Guid> _instanceRepository;

    public IisSiteAppService(
        IIisManager iisManager,
        IRepository<IisSiteEntity, Guid> siteRepository,
        IRepository<IisInstance, Guid> instanceRepository)
    {
        _iisManager = iisManager;
        _siteRepository = siteRepository;
        _instanceRepository = instanceRepository;
    }

    private async Task<string> GetConfigPathAsync(Guid iisInstanceId)
    {
        var instance = await _instanceRepository.GetAsync(iisInstanceId);
        return instance.ConfigPath;
    }

    public async Task<PagedResultDto<IisSiteDto>> GetListAsync(GetIisSiteListDto input)
    {
        var queryable = await _siteRepository.GetQueryableAsync();

        if (input.IisInstanceId.HasValue)
            queryable = queryable.Where(s => s.IisInstanceId == input.IisInstanceId.Value);

        if (!string.IsNullOrWhiteSpace(input.Filter))
            queryable = queryable.Where(s => s.SiteName.Contains(input.Filter));

        var totalCount = queryable.Count();
        var items = queryable.OrderBy(s => s.SiteName)
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
        var configPath = await GetConfigPathAsync(input.IisInstanceId);

        var existing = await _siteRepository.FindAsync(s =>
            s.SiteName == input.SiteName && s.IisInstanceId == input.IisInstanceId);
        if (existing != null)
            throw new UserFriendlyException($"站点 {input.SiteName} 已存在于此 IIS 实例中");

        var entity = ObjectMapper.Map<CreateIisSiteDto, IisSiteEntity>(input);
        entity.Status = "Unknown";

        try
        {
            entity = await _iisManager.CreateSiteAsync(configPath, entity);
            entity.Status = "Running";
        }
        catch (Exception ex)
        {
            Logger.LogWarning(ex, "Failed to create IIS site {SiteName}", input.SiteName);
            throw new UserFriendlyException($"创建 IIS 站点失败: {ex.Message}", details: ex.Message);
        }

        entity = await _siteRepository.InsertAsync(entity, autoSave: true);
        return ObjectMapper.Map<IisSiteEntity, IisSiteDto>(entity);
    }

    [Authorize(AppManagerPermissions.IisSites.Edit)]
    public async Task<IisSiteDto> UpdateAsync(Guid id, UpdateIisSiteDto input)
    {
        var entity = await _siteRepository.GetAsync(id);
        var configPath = await GetConfigPathAsync(entity.IisInstanceId);

        if (input.SiteName != null) entity.SiteName = input.SiteName;
        if (input.PhysicalPath != null) entity.PhysicalPath = input.PhysicalPath;
        if (input.Port.HasValue) entity.Port = input.Port.Value;
        if (input.IisInstanceId.HasValue) entity.IisInstanceId = input.IisInstanceId.Value;

        try
        {
            entity = await _iisManager.UpdateSiteAsync(configPath, entity);
        }
        catch (Exception ex)
        {
            Logger.LogWarning(ex, "Failed to update site {SiteName}", entity.SiteName);
            throw new UserFriendlyException($"更新站点 {entity.SiteName} 失败: {ex.Message}", details: ex.Message);
        }

        await _siteRepository.UpdateAsync(entity, autoSave: true);
        return ObjectMapper.Map<IisSiteEntity, IisSiteDto>(entity);
    }

    [Authorize(AppManagerPermissions.IisSites.Delete)]
    public async Task DeleteAsync(Guid id)
    {
        var entity = await _siteRepository.GetAsync(id);
        var configPath = await GetConfigPathAsync(entity.IisInstanceId);

        try
        {
            await _iisManager.DeleteSiteAsync(configPath, entity.SiteName);
        }
        catch (Exception ex)
        {
            Logger.LogWarning(ex, "Failed to delete site {SiteName}", entity.SiteName);
            throw new UserFriendlyException($"删除站点 {entity.SiteName} 失败: {ex.Message}", details: ex.Message);
        }
        await _siteRepository.DeleteAsync(entity, autoSave: true);
    }

    public async Task StartAsync(Guid id)
    {
        var entity = await _siteRepository.GetAsync(id);
        var configPath = await GetConfigPathAsync(entity.IisInstanceId);

        try
        {
            await _iisManager.StartSiteAsync(configPath, entity.SiteName);
        }
        catch (Exception ex)
        {
            Logger.LogWarning(ex, "Failed to start site {SiteName}", entity.SiteName);
            throw new UserFriendlyException($"启动站点 {entity.SiteName} 失败: {ex.Message}", details: ex.Message);
        }
        entity.Status = "Running";
        await _siteRepository.UpdateAsync(entity, autoSave: true);
    }

    public async Task StopAsync(Guid id)
    {
        var entity = await _siteRepository.GetAsync(id);
        var configPath = await GetConfigPathAsync(entity.IisInstanceId);

        try
        {
            await _iisManager.StopSiteAsync(configPath, entity.SiteName);
        }
        catch (Exception ex)
        {
            Logger.LogWarning(ex, "Failed to stop site {SiteName}", entity.SiteName);
            throw new UserFriendlyException($"停止站点 {entity.SiteName} 失败: {ex.Message}", details: ex.Message);
        }
        entity.Status = "Stopped";
        await _siteRepository.UpdateAsync(entity, autoSave: true);
    }

    [Authorize(AppManagerPermissions.IisSites.ManageBinding)]
    public async Task<List<SiteBindingDto>> GetBindingsAsync(Guid id)
    {
        var entity = await _siteRepository.GetAsync(id);
        var configPath = await GetConfigPathAsync(entity.IisInstanceId);
        var bindings = await _iisManager.GetSiteBindingsAsync(configPath, entity.SiteName);
        return ObjectMapper.Map<List<SiteBinding>, List<SiteBindingDto>>(bindings);
    }

    [Authorize(AppManagerPermissions.IisSites.ManageBinding)]
    public async Task AddBindingAsync(Guid id, SiteBindingDto input)
    {
        var entity = await _siteRepository.GetAsync(id);
        var configPath = await GetConfigPathAsync(entity.IisInstanceId);
        var binding = ObjectMapper.Map<SiteBindingDto, SiteBinding>(input);
        try
        {
            await _iisManager.AddBindingAsync(configPath, entity.SiteName, binding);
        }
        catch (Exception ex)
        {
            Logger.LogWarning(ex, "Failed to add binding for site {SiteName}", entity.SiteName);
            throw new UserFriendlyException($"添加绑定失败: {ex.Message}", details: ex.Message);
        }
        var bindings = await _iisManager.GetSiteBindingsAsync(configPath, entity.SiteName);
        entity.BindingsJson = System.Text.Json.JsonSerializer.Serialize(bindings);
        await _siteRepository.UpdateAsync(entity, autoSave: true);
    }

    [Authorize(AppManagerPermissions.IisSites.ManageBinding)]
    public async Task RemoveBindingAsync(Guid id, SiteBindingDto input)
    {
        var entity = await _siteRepository.GetAsync(id);
        var configPath = await GetConfigPathAsync(entity.IisInstanceId);
        var binding = ObjectMapper.Map<SiteBindingDto, SiteBinding>(input);
        try
        {
            await _iisManager.RemoveBindingAsync(configPath, entity.SiteName, binding);
        }
        catch (Exception ex)
        {
            Logger.LogWarning(ex, "Failed to remove binding for site {SiteName}", entity.SiteName);
            throw new UserFriendlyException($"移除绑定失败: {ex.Message}", details: ex.Message);
        }
        var bindings = await _iisManager.GetSiteBindingsAsync(configPath, entity.SiteName);
        entity.BindingsJson = System.Text.Json.JsonSerializer.Serialize(bindings);
        await _siteRepository.UpdateAsync(entity, autoSave: true);
    }

    [Authorize(AppManagerPermissions.IisSites.ManageAppPool)]
    public async Task<AppPoolConfigDto> GetAppPoolConfigAsync(Guid id)
    {
        var entity = await _siteRepository.GetAsync(id);
        if (entity.AppPoolName == null) return new AppPoolConfigDto();
        var configPath = await GetConfigPathAsync(entity.IisInstanceId);
        var config = await _iisManager.GetAppPoolConfigAsync(configPath, entity.AppPoolName);
        return ObjectMapper.Map<AppPoolConfig, AppPoolConfigDto>(config);
    }

    [Authorize(AppManagerPermissions.IisSites.ManageAppPool)]
    public async Task UpdateAppPoolConfigAsync(Guid id, AppPoolConfigDto input)
    {
        var entity = await _siteRepository.GetAsync(id);
        if (entity.AppPoolName == null) return;
        var configPath = await GetConfigPathAsync(entity.IisInstanceId);
        var config = ObjectMapper.Map<AppPoolConfigDto, AppPoolConfig>(input);
        try
        {
            await _iisManager.UpdateAppPoolConfigAsync(configPath, entity.AppPoolName, config);
        }
        catch (Exception ex)
        {
            Logger.LogWarning(ex, "Failed to update app pool config for {PoolName}", entity.AppPoolName);
            throw new UserFriendlyException($"更新应用池配置失败: {ex.Message}", details: ex.Message);
        }
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
        var configPath = await GetConfigPathAsync(entity.IisInstanceId);
        try
        {
            await _iisManager.AddSubApplicationAsync(configPath, entity.SiteName,
                ObjectMapper.Map<SubApplicationDto, SubApplication>(input));
        }
        catch (Exception ex)
        {
            Logger.LogWarning(ex, "Failed to add sub-application for site {SiteName}", entity.SiteName);
            throw new UserFriendlyException($"添加子应用失败: {ex.Message}", details: ex.Message);
        }
        var apps = await GetSubApplicationsAsync(id);
        apps.Add(input);
        entity.SubApplicationsJson = System.Text.Json.JsonSerializer.Serialize(apps);
        await _siteRepository.UpdateAsync(entity, autoSave: true);
    }

    public async Task RemoveSubApplicationAsync(Guid id, string alias)
    {
        var entity = await _siteRepository.GetAsync(id);
        var configPath = await GetConfigPathAsync(entity.IisInstanceId);
        try
        {
            await _iisManager.RemoveSubApplicationAsync(configPath, entity.SiteName, alias);
        }
        catch (Exception ex)
        {
            Logger.LogWarning(ex, "Failed to remove sub-application for site {SiteName}", entity.SiteName);
            throw new UserFriendlyException($"移除子应用失败: {ex.Message}", details: ex.Message);
        }
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
        var configPath = await GetConfigPathAsync(entity.IisInstanceId);
        try
        {
            await _iisManager.AddVirtualDirectoryAsync(configPath, entity.SiteName,
                ObjectMapper.Map<VirtualDirectoryDto, VirtualDirectory>(input));
        }
        catch (Exception ex)
        {
            Logger.LogWarning(ex, "Failed to add virtual directory for site {SiteName}", entity.SiteName);
            throw new UserFriendlyException($"添加虚拟目录失败: {ex.Message}", details: ex.Message);
        }
        var vdirs = await GetVirtualDirectoriesAsync(id);
        vdirs.Add(input);
        entity.VirtualDirectoriesJson = System.Text.Json.JsonSerializer.Serialize(vdirs);
        await _siteRepository.UpdateAsync(entity, autoSave: true);
    }

    public async Task RemoveVirtualDirectoryAsync(Guid id, string alias)
    {
        var entity = await _siteRepository.GetAsync(id);
        var configPath = await GetConfigPathAsync(entity.IisInstanceId);
        try
        {
            await _iisManager.RemoveVirtualDirectoryAsync(configPath, entity.SiteName, alias);
        }
        catch (Exception ex)
        {
            Logger.LogWarning(ex, "Failed to remove virtual directory for site {SiteName}", entity.SiteName);
            throw new UserFriendlyException($"移除虚拟目录失败: {ex.Message}", details: ex.Message);
        }
        var vdirs = await GetVirtualDirectoriesAsync(id);
        vdirs.RemoveAll(v => v.Alias == alias);
        entity.VirtualDirectoriesJson = System.Text.Json.JsonSerializer.Serialize(vdirs);
        await _siteRepository.UpdateAsync(entity, autoSave: true);
    }

    [Authorize(AppManagerPermissions.IisSites.ManagePermissions)]
    public async Task<List<NtfsPermissionDto>> GetNtfsPermissionsAsync(Guid id)
    {
        var entity = await _siteRepository.GetAsync(id);
        var configPath = await GetConfigPathAsync(entity.IisInstanceId);
        return await _iisManager.GetNtfsPermissionsAsync(configPath, entity.PhysicalPath) is { } perms
            ? ObjectMapper.Map<List<NtfsPermission>, List<NtfsPermissionDto>>(perms)
            : new List<NtfsPermissionDto>();
    }

    [Authorize(AppManagerPermissions.IisSites.ManagePermissions)]
    public async Task SetNtfsPermissionAsync(Guid id, NtfsPermissionDto input)
    {
        var entity = await _siteRepository.GetAsync(id);
        var configPath = await GetConfigPathAsync(entity.IisInstanceId);
        var perm = ObjectMapper.Map<NtfsPermissionDto, NtfsPermission>(input);
        try
        {
            await _iisManager.SetNtfsPermissionAsync(configPath, entity.PhysicalPath, perm);
        }
        catch (Exception ex)
        {
            Logger.LogWarning(ex, "Failed to set NTFS permission for site {SiteName}", entity.SiteName);
            throw new UserFriendlyException($"设置 NTFS 权限失败: {ex.Message}", details: ex.Message);
        }
    }

    [Authorize(AppManagerPermissions.IisSites.ManagePermissions)]
    public async Task RemoveNtfsPermissionAsync(Guid id, string identity)
    {
        var entity = await _siteRepository.GetAsync(id);
        var configPath = await GetConfigPathAsync(entity.IisInstanceId);
        try
        {
            await _iisManager.RemoveNtfsPermissionAsync(configPath, entity.PhysicalPath, identity);
        }
        catch (Exception ex)
        {
            Logger.LogWarning(ex, "Failed to remove NTFS permission for site {SiteName}", entity.SiteName);
            throw new UserFriendlyException($"移除 NTFS 权限失败: {ex.Message}", details: ex.Message);
        }
    }
}
