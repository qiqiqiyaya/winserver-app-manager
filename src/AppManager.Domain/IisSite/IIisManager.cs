using System.Collections.Generic;
using System.Threading.Tasks;
using AppManager.Models;
using IisSiteEntity = AppManager.IisSite.IisSite;
using Volo.Abp.DependencyInjection;

namespace AppManager.IisSite;

public interface IIisManager : ITransientDependency
{
    Task<List<IisSiteEntity>> GetAllSitesAsync(string configPath);
    Task<IisSiteEntity?> GetSiteByNameAsync(string configPath, string siteName);
    Task<IisSiteEntity> CreateSiteAsync(string configPath, IisSiteEntity site);
    Task<IisSiteEntity> UpdateSiteAsync(string configPath, IisSiteEntity site);
    Task DeleteSiteAsync(string configPath, string siteName);
    Task StartSiteAsync(string configPath, string siteName);
    Task StopSiteAsync(string configPath, string siteName);
    Task<List<SiteBinding>> GetSiteBindingsAsync(string configPath, string siteName);
    Task AddBindingAsync(string configPath, string siteName, SiteBinding binding);
    Task RemoveBindingAsync(string configPath, string siteName, SiteBinding binding);
    Task<AppPoolConfig> GetAppPoolConfigAsync(string configPath, string poolName);
    Task UpdateAppPoolConfigAsync(string configPath, string poolName, AppPoolConfig config);
    Task AddSubApplicationAsync(string configPath, string siteName, SubApplication app);
    Task RemoveSubApplicationAsync(string configPath, string siteName, string alias);
    Task AddVirtualDirectoryAsync(string configPath, string siteName, VirtualDirectory vdir);
    Task RemoveVirtualDirectoryAsync(string configPath, string siteName, string alias);
    Task<List<NtfsPermission>> GetNtfsPermissionsAsync(string configPath, string physicalPath);
    Task SetNtfsPermissionAsync(string configPath, string physicalPath, NtfsPermission permission);
    Task RemoveNtfsPermissionAsync(string configPath, string physicalPath, string identity);
}
