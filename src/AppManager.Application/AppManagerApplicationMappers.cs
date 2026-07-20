using AppManager.Backups;
using AppManager.IisSite;
using AppManager.IisSites;
using AppManager.Models;
using AppManager.WindowsServices;
using Riok.Mapperly.Abstractions;
using System.Collections.Generic;
using System.Text.Json;
using Volo.Abp.Mapperly;
using IisSiteEntity = AppManager.IisSite.IisSite;
using WindowsServiceEntity = AppManager.WindowsService.WindowsService;

namespace AppManager;

// ============================================================
// IIS Site Mappings
// ============================================================

[Mapper]
public partial class IisSiteToDtoMapper : MapperBase<IisSiteEntity, IisSiteDto>
{
    [MapProperty(new[] { nameof(IisSiteEntity.BindingsJson) }, nameof(IisSiteDto.Bindings),
                 Use = nameof(DeserializeBindings))]
    [MapProperty(new[] { nameof(IisSiteEntity.AppPoolConfigJson) }, nameof(IisSiteDto.AppPoolConfig),
                 Use = nameof(DeserializeAppPoolConfig))]
    [MapProperty(new[] { nameof(IisSiteEntity.SubApplicationsJson) }, nameof(IisSiteDto.SubApplications),
                 Use = nameof(DeserializeSubApplications))]
    [MapProperty(new[] { nameof(IisSiteEntity.VirtualDirectoriesJson) }, nameof(IisSiteDto.VirtualDirectories),
                 Use = nameof(DeserializeVirtualDirectories))]
    [MapProperty(new[] { nameof(IisSiteEntity.NtfsPermissionsJson) }, nameof(IisSiteDto.NtfsPermissions),
                 Use = nameof(DeserializeNtfsPermissions))]
    public override partial IisSiteDto Map(IisSiteEntity source);

    public override partial void Map(IisSiteEntity source, IisSiteDto destination);

    private List<SiteBindingDto> DeserializeBindings(string? json)
        => json != null ? JsonSerializer.Deserialize<List<SiteBindingDto>>(json) ?? [] : [];

    private AppPoolConfigDto? DeserializeAppPoolConfig(string? json)
        => json != null ? JsonSerializer.Deserialize<AppPoolConfigDto>(json) : null;

    private List<SubApplicationDto> DeserializeSubApplications(string? json)
        => json != null ? JsonSerializer.Deserialize<List<SubApplicationDto>>(json) ?? [] : [];

    private List<VirtualDirectoryDto> DeserializeVirtualDirectories(string? json)
        => json != null ? JsonSerializer.Deserialize<List<VirtualDirectoryDto>>(json) ?? [] : [];

    private List<NtfsPermissionDto> DeserializeNtfsPermissions(string? json)
        => json != null ? JsonSerializer.Deserialize<List<NtfsPermissionDto>>(json) ?? [] : [];
}

[Mapper]
public partial class CreateIisSiteDtoToEntityMapper : MapperBase<CreateIisSiteDto, IisSiteEntity>
{
    [MapProperty(new[] { nameof(CreateIisSiteDto.Bindings) }, nameof(IisSiteEntity.BindingsJson),
                 Use = nameof(SerializeBindings))]
    [MapProperty(new[] { nameof(CreateIisSiteDto.AppPoolConfig) }, nameof(IisSiteEntity.AppPoolConfigJson),
                 Use = nameof(SerializeAppPoolConfig))]
    public override partial IisSiteEntity Map(CreateIisSiteDto source);

    public override partial void Map(CreateIisSiteDto source, IisSiteEntity destination);

    private string? SerializeBindings(List<SiteBindingDto>? bindings)
        => bindings != null ? JsonSerializer.Serialize(bindings) : null;

    private string? SerializeAppPoolConfig(AppPoolConfigDto? config)
        => config != null ? JsonSerializer.Serialize(config) : null;
}

// ============================================================
// Windows Service Mappings
// ============================================================

[Mapper]
public partial class WindowsServiceToDtoMapper : MapperBase<WindowsServiceEntity, WindowsServiceDto>
{
    [MapProperty(new[] { nameof(WindowsServiceEntity.FailureActionsJson) },
                 nameof(WindowsServiceDto.FailureActions), Use = nameof(DeserializeFailureActions))]
    [MapProperty(new[] { nameof(WindowsServiceEntity.DependenciesJson) },
                 nameof(WindowsServiceDto.Dependencies), Use = nameof(DeserializeDependencies))]
    public override partial WindowsServiceDto Map(WindowsServiceEntity source);

    public override partial void Map(WindowsServiceEntity source, WindowsServiceDto destination);

    private List<FailureActionDto>? DeserializeFailureActions(string? json)
        => json != null ? JsonSerializer.Deserialize<List<FailureActionDto>>(json) : null;

    private List<string>? DeserializeDependencies(string? json)
        => json != null ? JsonSerializer.Deserialize<List<string>>(json) : null;
}

[Mapper]
public partial class CreateWindowsServiceDtoToEntityMapper : MapperBase<CreateWindowsServiceDto, WindowsServiceEntity>
{
    [MapProperty(new[] { nameof(CreateWindowsServiceDto.FailureActions) },
                 nameof(WindowsServiceEntity.FailureActionsJson), Use = nameof(SerializeFailureActions))]
    [MapProperty(new[] { nameof(CreateWindowsServiceDto.Dependencies) },
                 nameof(WindowsServiceEntity.DependenciesJson), Use = nameof(SerializeDependencies))]
    public override partial WindowsServiceEntity Map(CreateWindowsServiceDto source);

    public override partial void Map(CreateWindowsServiceDto source, WindowsServiceEntity destination);

    private string? SerializeFailureActions(List<FailureActionDto>? actions)
        => actions != null ? JsonSerializer.Serialize(actions) : null;

    private string? SerializeDependencies(List<string>? deps)
        => deps != null ? JsonSerializer.Serialize(deps) : null;
}

// ============================================================
// Backup Mappings
// ============================================================

[Mapper]
public partial class IisSiteBackupToDtoMapper : MapperBase<IisSiteBackup, IisSiteBackupDto>
{
    public override partial IisSiteBackupDto Map(IisSiteBackup source);
    public override partial void Map(IisSiteBackup source, IisSiteBackupDto destination);
}

[Mapper]
public partial class WindowsServiceBackupToDtoMapper : MapperBase<WindowsServiceBackup, WindowsServiceBackupDto>
{
    public override partial WindowsServiceBackupDto Map(WindowsServiceBackup source);
    public override partial void Map(WindowsServiceBackup source, WindowsServiceBackupDto destination);
}

// ============================================================
// IIS Instance Mappings
// ============================================================

[Mapper]
public partial class IisInstanceToDtoMapper : MapperBase<IisInstance, IisInstanceDto>
{
    public override partial IisInstanceDto Map(IisInstance source);
    public override partial void Map(IisInstance source, IisInstanceDto destination);
}

[Mapper]
public partial class CreateIisInstanceDtoToEntityMapper : MapperBase<CreateIisInstanceDto, IisInstance>
{
    public override partial IisInstance Map(CreateIisInstanceDto source);
    public override partial void Map(CreateIisInstanceDto source, IisInstance destination);
}

// ============================================================
// Value Object Mappings (DTO ↔ Domain Model)
// ============================================================

[Mapper]
public partial class SiteBindingDtoToModelMapper : MapperBase<SiteBindingDto, SiteBinding>
{
    public override partial SiteBinding Map(SiteBindingDto source);
    public override partial void Map(SiteBindingDto source, SiteBinding destination);
}

[Mapper]
public partial class SiteBindingModelToDtoMapper : MapperBase<SiteBinding, SiteBindingDto>
{
    public override partial SiteBindingDto Map(SiteBinding source);
    public override partial void Map(SiteBinding source, SiteBindingDto destination);
}

[Mapper]
public partial class AppPoolConfigDtoToModelMapper : MapperBase<AppPoolConfigDto, AppPoolConfig>
{
    public override partial AppPoolConfig Map(AppPoolConfigDto source);
    public override partial void Map(AppPoolConfigDto source, AppPoolConfig destination);
}

[Mapper]
public partial class AppPoolConfigModelToDtoMapper : MapperBase<AppPoolConfig, AppPoolConfigDto>
{
    public override partial AppPoolConfigDto Map(AppPoolConfig source);
    public override partial void Map(AppPoolConfig source, AppPoolConfigDto destination);
}

[Mapper]
public partial class SubApplicationDtoToModelMapper : MapperBase<SubApplicationDto, SubApplication>
{
    public override partial SubApplication Map(SubApplicationDto source);
    public override partial void Map(SubApplicationDto source, SubApplication destination);
}

[Mapper]
public partial class SubApplicationModelToDtoMapper : MapperBase<SubApplication, SubApplicationDto>
{
    public override partial SubApplicationDto Map(SubApplication source);
    public override partial void Map(SubApplication source, SubApplicationDto destination);
}

[Mapper]
public partial class VirtualDirectoryDtoToModelMapper : MapperBase<VirtualDirectoryDto, VirtualDirectory>
{
    public override partial VirtualDirectory Map(VirtualDirectoryDto source);
    public override partial void Map(VirtualDirectoryDto source, VirtualDirectory destination);
}

[Mapper]
public partial class VirtualDirectoryModelToDtoMapper : MapperBase<VirtualDirectory, VirtualDirectoryDto>
{
    public override partial VirtualDirectoryDto Map(VirtualDirectory source);
    public override partial void Map(VirtualDirectory source, VirtualDirectoryDto destination);
}

[Mapper]
public partial class NtfsPermissionDtoToModelMapper : MapperBase<NtfsPermissionDto, NtfsPermission>
{
    public override partial NtfsPermission Map(NtfsPermissionDto source);
    public override partial void Map(NtfsPermissionDto source, NtfsPermission destination);
}

[Mapper]
public partial class NtfsPermissionModelToDtoMapper : MapperBase<NtfsPermission, NtfsPermissionDto>
{
    public override partial NtfsPermissionDto Map(NtfsPermission source);
    public override partial void Map(NtfsPermission source, NtfsPermissionDto destination);
}

[Mapper]
public partial class FailureActionDtoToModelMapper : MapperBase<FailureActionDto, FailureAction>
{
    public override partial FailureAction Map(FailureActionDto source);
    public override partial void Map(FailureActionDto source, FailureAction destination);
}

[Mapper]
public partial class FailureActionModelToDtoMapper : MapperBase<FailureAction, FailureActionDto>
{
    public override partial FailureActionDto Map(FailureAction source);
    public override partial void Map(FailureAction source, FailureActionDto destination);
}