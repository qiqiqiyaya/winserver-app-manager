using System.Collections.Generic;
using System.Threading.Tasks;
using AppManager.Models;
using Volo.Abp.DependencyInjection;

namespace AppManager.Services;

public interface IIisManager : ITransientDependency
{
    Task<List<IisSite.IisSite>> GetAllSitesAsync();
    Task<IisSite.IisSite?> GetSiteByNameAsync(string siteName);
    Task<IisSite.IisSite> CreateSiteAsync(IisSite.IisSite site);
    Task<IisSite.IisSite> UpdateSiteAsync(IisSite.IisSite site);
    Task DeleteSiteAsync(string siteName);
    Task StartSiteAsync(string siteName);
    Task StopSiteAsync(string siteName);
    Task<List<SiteBinding>> GetSiteBindingsAsync(string siteName);
    Task AddBindingAsync(string siteName, SiteBinding binding);
    Task RemoveBindingAsync(string siteName, SiteBinding binding);
    Task<AppPoolConfig> GetAppPoolConfigAsync(string poolName);
    Task UpdateAppPoolConfigAsync(string poolName, AppPoolConfig config);
    Task AddSubApplicationAsync(string siteName, SubApplication app);
    Task RemoveSubApplicationAsync(string siteName, string alias);
    Task AddVirtualDirectoryAsync(string siteName, VirtualDirectory vdir);
    Task RemoveVirtualDirectoryAsync(string siteName, string alias);
    Task<List<NtfsPermission>> GetNtfsPermissionsAsync(string physicalPath);
    Task SetNtfsPermissionAsync(string physicalPath, NtfsPermission permission);
    Task RemoveNtfsPermissionAsync(string physicalPath, string identity);
}
