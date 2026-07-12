using System;
using Volo.Abp.Application.Dtos;

namespace AppManager.IisSites;

public class IisInstanceDto : EntityDto<Guid>
{
    public string Name { get; set; } = string.Empty;
    public string ConfigPath { get; set; } = string.Empty;
    public string Status { get; set; } = "Unknown";
}
