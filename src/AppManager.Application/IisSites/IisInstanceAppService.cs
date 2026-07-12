using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AppManager.IisSite;
using AppManager.IisSites;
using AppManager.Permissions;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Repositories;

namespace AppManager.Application.IisSites;

[Authorize(AppManagerPermissions.IisInstances.Default)]
public class IisInstanceAppService : AppManagerAppService, IIisInstanceAppService
{
    private readonly IRepository<IisInstance, Guid> _instanceRepository;

    public IisInstanceAppService(IRepository<IisInstance, Guid> instanceRepository)
    {
        _instanceRepository = instanceRepository;
    }

    public async Task<PagedResultDto<IisInstanceDto>> GetListAsync(PagedAndSortedResultRequestDto input)
    {
        var query = await _instanceRepository.GetQueryableAsync();
        var totalCount = query.Count();
        var items = query.OrderBy(i => i.Name)
            .Skip(input.SkipCount).Take(input.MaxResultCount).ToList();

        return new PagedResultDto<IisInstanceDto>(
            totalCount,
            ObjectMapper.Map<List<IisInstance>, List<IisInstanceDto>>(items));
    }

    public async Task<IisInstanceDto> GetAsync(Guid id)
    {
        var entity = await _instanceRepository.GetAsync(id);
        return ObjectMapper.Map<IisInstance, IisInstanceDto>(entity);
    }

    [Authorize(AppManagerPermissions.IisInstances.Create)]
    public async Task<IisInstanceDto> CreateAsync(CreateIisInstanceDto input)
    {
        var existing = await _instanceRepository.FindAsync(i => i.ConfigPath == input.ConfigPath);
        if (existing != null)
            throw new UserFriendlyException($"IIS 实例 {input.Name} 已存在");

        var entity = ObjectMapper.Map<CreateIisInstanceDto, IisInstance>(input);
        entity.Status = "Unknown";
        entity = await _instanceRepository.InsertAsync(entity, autoSave: true);
        return ObjectMapper.Map<IisInstance, IisInstanceDto>(entity);
    }

    [Authorize(AppManagerPermissions.IisInstances.Edit)]
    public async Task<IisInstanceDto> UpdateAsync(Guid id, UpdateIisInstanceDto input)
    {
        var entity = await _instanceRepository.GetAsync(id);
        if (input.Name != null) entity.Name = input.Name;
        if (input.ConfigPath != null) entity.ConfigPath = input.ConfigPath;
        entity = await _instanceRepository.UpdateAsync(entity, autoSave: true);
        return ObjectMapper.Map<IisInstance, IisInstanceDto>(entity);
    }

    [Authorize(AppManagerPermissions.IisInstances.Delete)]
    public async Task DeleteAsync(Guid id)
    {
        await _instanceRepository.DeleteAsync(id);
    }
}
