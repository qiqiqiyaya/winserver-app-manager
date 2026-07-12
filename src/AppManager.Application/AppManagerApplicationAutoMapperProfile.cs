using System.Collections.Generic;
using System.Text.Json;
using AppManager.IisSite;
using IisSiteEntity = AppManager.IisSite.IisSite;
using AppManager.WindowsService;
using WindowsServiceEntity = AppManager.WindowsService.WindowsService;
using AppManager.IisSites;
using AppManager.WindowsServices;
using AppManager.Backups;
using AppManager.Models;
using AutoMapper;

namespace AppManager;

public class AppManagerApplicationAutoMapperProfile : Profile
{
    public AppManagerApplicationAutoMapperProfile()
    {
        /* IIS 站点映射 */
        CreateMap<IisSiteEntity, IisSiteDto>()
            .ForMember(d => d.IisInstanceName, o => o.MapFrom(s =>
                s.IisInstance != null ? s.IisInstance.Name : null))
            .ForMember(d => d.Bindings, o => o.MapFrom(s =>
                s.BindingsJson != null
                    ? JsonSerializer.Deserialize<List<SiteBindingDto>>(s.BindingsJson)
                    : new List<SiteBindingDto>()))
            .ForMember(d => d.AppPoolConfig, o => o.MapFrom(s =>
                s.AppPoolConfigJson != null
                    ? JsonSerializer.Deserialize<AppPoolConfigDto>(s.AppPoolConfigJson)
                    : null))
            .ForMember(d => d.SubApplications, o => o.MapFrom(s =>
                s.SubApplicationsJson != null
                    ? JsonSerializer.Deserialize<List<SubApplicationDto>>(s.SubApplicationsJson)
                    : new List<SubApplicationDto>()))
            .ForMember(d => d.VirtualDirectories, o => o.MapFrom(s =>
                s.VirtualDirectoriesJson != null
                    ? JsonSerializer.Deserialize<List<VirtualDirectoryDto>>(s.VirtualDirectoriesJson)
                    : new List<VirtualDirectoryDto>()))
            .ForMember(d => d.NtfsPermissions, o => o.MapFrom(s =>
                s.NtfsPermissionsJson != null
                    ? JsonSerializer.Deserialize<List<NtfsPermissionDto>>(s.NtfsPermissionsJson)
                    : new List<NtfsPermissionDto>()));

        CreateMap<CreateIisSiteDto, IisSiteEntity>()
            .ForMember(d => d.Id, o => o.Ignore())
            .ForMember(d => d.BindingsJson, o => o.MapFrom(s =>
                s.Bindings != null ? JsonSerializer.Serialize(s.Bindings) : null))
            .ForMember(d => d.AppPoolConfigJson, o => o.MapFrom(s =>
                s.AppPoolConfig != null ? JsonSerializer.Serialize(s.AppPoolConfig) : null));

        /* Windows 服务映射 */
        CreateMap<WindowsServiceEntity, WindowsServiceDto>()
            .ForMember(d => d.FailureActions, o => o.MapFrom(s =>
                s.FailureActionsJson != null
                    ? JsonSerializer.Deserialize<List<FailureActionDto>>(s.FailureActionsJson)
                    : null))
            .ForMember(d => d.Dependencies, o => o.MapFrom(s =>
                s.DependenciesJson != null
                    ? JsonSerializer.Deserialize<List<string>>(s.DependenciesJson)
                    : null));

        CreateMap<CreateWindowsServiceDto, WindowsServiceEntity>()
            .ForMember(d => d.Id, o => o.Ignore())
            .ForMember(d => d.FailureActionsJson, o => o.MapFrom(s =>
                s.FailureActions != null ? JsonSerializer.Serialize(s.FailureActions) : null))
            .ForMember(d => d.DependenciesJson, o => o.MapFrom(s =>
                s.Dependencies != null ? JsonSerializer.Serialize(s.Dependencies) : null));

        /* 备份映射 */
        CreateMap<IisSiteBackup, IisSiteBackupDto>();
        CreateMap<WindowsServiceBackup, WindowsServiceBackupDto>();

        /* IIS 实例映射 */
        CreateMap<IisInstance, IisInstanceDto>();
        CreateMap<CreateIisInstanceDto, IisInstance>()
            .ForMember(d => d.Id, o => o.Ignore());

        /* 值对象映射 */
        CreateMap<SiteBindingDto, SiteBinding>().ReverseMap();
        CreateMap<AppPoolConfigDto, AppPoolConfig>().ReverseMap();
        CreateMap<SubApplicationDto, SubApplication>().ReverseMap();
        CreateMap<VirtualDirectoryDto, VirtualDirectory>().ReverseMap();
        CreateMap<NtfsPermissionDto, NtfsPermission>().ReverseMap();
        CreateMap<FailureActionDto, FailureAction>().ReverseMap();
    }
}
