using System;
using System.Collections.Generic;
using Volo.Abp.Application.Dtos;

namespace AppManager.AuditLogs;

public class AuditLogDetailDto : AuditLogDto
{
    public List<EntityChangeDto> EntityChanges { get; set; } = new();
}

public class EntityChangeDto
{
    public string? EntityTypeFullName { get; set; }
    public string? ChangeType { get; set; }
    public List<EntityPropertyChangeDto> PropertyChanges { get; set; } = new();
}

public class EntityPropertyChangeDto
{
    public string? PropertyName { get; set; }
    public string? OriginalValue { get; set; }
    public string? NewValue { get; set; }
}
