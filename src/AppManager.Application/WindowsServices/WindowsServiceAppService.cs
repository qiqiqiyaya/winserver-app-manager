using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AppManager.WindowsService;
using WindowsServiceEntity = AppManager.WindowsService.WindowsService;
using AppManager.WindowsServices;
using AppManager.Permissions;
using AppManager.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Repositories;

namespace AppManager.Application.WindowsServices;

[Authorize(AppManagerPermissions.WindowsServices.Default)]
public class WindowsServiceAppService : AppManagerAppService, IWindowsServiceAppService
{
    private readonly IWindowsServiceManager _serviceManager;
    private readonly IRepository<WindowsServiceEntity, Guid> _serviceRepository;

    public WindowsServiceAppService(
        IWindowsServiceManager serviceManager,
        IRepository<WindowsServiceEntity, Guid> serviceRepository)
    {
        _serviceManager = serviceManager;
        _serviceRepository = serviceRepository;
    }

    public async Task<PagedResultDto<WindowsServiceDto>> GetListAsync(GetWindowsServiceListDto input)
    {
        var query = string.IsNullOrWhiteSpace(input.Filter)
            ? await _serviceRepository.GetQueryableAsync()
            : (await _serviceRepository.GetQueryableAsync())
                .Where(s => s.ServiceName.Contains(input.Filter)
                            || s.DisplayName.Contains(input.Filter));

        var totalCount = query.Count();
        var items = query.OrderBy(s => s.ServiceName)
            .Skip(input.SkipCount).Take(input.MaxResultCount).ToList();

        return new PagedResultDto<WindowsServiceDto>(
            totalCount,
            ObjectMapper.Map<List<WindowsServiceEntity>, List<WindowsServiceDto>>(items));
    }

    public async Task<WindowsServiceDto> GetAsync(Guid id)
    {
        var entity = await _serviceRepository.GetAsync(id);
        return ObjectMapper.Map<WindowsServiceEntity, WindowsServiceDto>(entity);
    }

    [Authorize(AppManagerPermissions.WindowsServices.Create)]
    public async Task<WindowsServiceDto> CreateAsync(CreateWindowsServiceDto input)
    {
        var existing = await _serviceRepository.FindAsync(s => s.ServiceName == input.ServiceName);
        if (existing != null)
            throw new BusinessException(AppManagerDomainErrorCodes.ServiceAlreadyExists);

        var entity = ObjectMapper.Map<CreateWindowsServiceDto, WindowsServiceEntity>(input);
        try { entity = await _serviceManager.CreateServiceAsync(entity); entity.Status = "Running"; }
        catch (Exception ex) { Logger.LogWarning(ex, "Failed to create service {ServiceName}", input.ServiceName); entity.Status = "Unknown"; }

        entity = await _serviceRepository.InsertAsync(entity, autoSave: true);
        return ObjectMapper.Map<WindowsServiceEntity, WindowsServiceDto>(entity);
    }

    [Authorize(AppManagerPermissions.WindowsServices.Edit)]
    public async Task<WindowsServiceDto> UpdateAsync(Guid id, UpdateWindowsServiceDto input)
    {
        var entity = await _serviceRepository.GetAsync(id);
        if (input.DisplayName != null) entity.DisplayName = input.DisplayName;
        if (input.Description != null) entity.Description = input.Description;
        if (input.StartType != null) entity.StartType = input.StartType;
        if (input.Account != null) entity.Account = input.Account;
        if (input.ExecutablePath != null) entity.ExecutablePath = input.ExecutablePath;
        try { await _serviceManager.UpdateServiceAsync(entity); }
        catch (Exception ex) { Logger.LogWarning(ex, "Failed to update service {ServiceName}", entity.ServiceName); }
        await _serviceRepository.UpdateAsync(entity, autoSave: true);
        return ObjectMapper.Map<WindowsServiceEntity, WindowsServiceDto>(entity);
    }

    [Authorize(AppManagerPermissions.WindowsServices.Delete)]
    public async Task DeleteAsync(Guid id)
    {
        var entity = await _serviceRepository.GetAsync(id);
        try { await _serviceManager.DeleteServiceAsync(entity.ServiceName); }
        catch (Exception ex) { Logger.LogWarning(ex, "Failed to delete service {ServiceName}", entity.ServiceName); }
        await _serviceRepository.DeleteAsync(entity, autoSave: true);
    }

    [Authorize(AppManagerPermissions.WindowsServices.Start)]
    public async Task StartAsync(Guid id)
    {
        var entity = await _serviceRepository.GetAsync(id);
        await _serviceManager.StartServiceAsync(entity.ServiceName);
        entity.Status = "Running";
        await _serviceRepository.UpdateAsync(entity, autoSave: true);
    }

    [Authorize(AppManagerPermissions.WindowsServices.Stop)]
    public async Task StopAsync(Guid id)
    {
        var entity = await _serviceRepository.GetAsync(id);
        await _serviceManager.StopServiceAsync(entity.ServiceName);
        entity.Status = "Stopped";
        await _serviceRepository.UpdateAsync(entity, autoSave: true);
    }

    [Authorize(AppManagerPermissions.WindowsServices.Restart)]
    public async Task RestartAsync(Guid id)
    {
        var entity = await _serviceRepository.GetAsync(id);
        await _serviceManager.RestartServiceAsync(entity.ServiceName);
        entity.Status = "Running";
        await _serviceRepository.UpdateAsync(entity, autoSave: true);
    }
}
