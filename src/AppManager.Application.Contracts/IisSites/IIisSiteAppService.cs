using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace AppManager.IisSites;

public interface IIisSiteAppService : IApplicationService
{
    Task<PagedResultDto<IisSiteDto>> GetListAsync(GetIisSiteListDto input);
    Task<IisSiteDto> GetAsync(Guid id);
    Task<IisSiteDto> CreateAsync(CreateIisSiteDto input);
    Task<IisSiteDto> UpdateAsync(Guid id, UpdateIisSiteDto input);
    Task DeleteAsync(Guid id);
    Task StartAsync(Guid id);
    Task StopAsync(Guid id);
    Task<List<SiteBindingDto>> GetBindingsAsync(Guid id);
    Task AddBindingAsync(Guid id, SiteBindingDto input);
    Task RemoveBindingAsync(Guid id, SiteBindingDto input);
    Task<AppPoolConfigDto> GetAppPoolConfigAsync(Guid id);
    Task UpdateAppPoolConfigAsync(Guid id, AppPoolConfigDto input);
    Task<List<SubApplicationDto>> GetSubApplicationsAsync(Guid id);
    Task AddSubApplicationAsync(Guid id, SubApplicationDto input);
    Task RemoveSubApplicationAsync(Guid id, string alias);
    Task<List<VirtualDirectoryDto>> GetVirtualDirectoriesAsync(Guid id);
    Task AddVirtualDirectoryAsync(Guid id, VirtualDirectoryDto input);
    Task RemoveVirtualDirectoryAsync(Guid id, string alias);
    Task<List<NtfsPermissionDto>> GetNtfsPermissionsAsync(Guid id);
    Task SetNtfsPermissionAsync(Guid id, NtfsPermissionDto input);
    Task RemoveNtfsPermissionAsync(Guid id, string identity);
}
