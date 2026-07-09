using AppManager.WindowsService;
using WindowsServiceEntity = AppManager.WindowsService.WindowsService;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Volo.Abp.EntityFrameworkCore.Modeling;

namespace AppManager.EntityFrameworkCore.Configurations;

public class WindowsServiceConfiguration : IEntityTypeConfiguration<WindowsServiceEntity>
{
    public void Configure(EntityTypeBuilder<WindowsServiceEntity> builder)
    {
        builder.ToTable(AppManagerConsts.DbTablePrefix + "WindowsServices", AppManagerConsts.DbSchema);
        builder.ConfigureByConvention();

        builder.Property(x => x.ServiceName).IsRequired().HasMaxLength(256);
        builder.HasIndex(x => x.ServiceName).IsUnique();

        builder.Property(x => x.DisplayName).IsRequired().HasMaxLength(256);
        builder.Property(x => x.Description).HasMaxLength(1024);
        builder.Property(x => x.ExecutablePath).IsRequired().HasMaxLength(1024);
        builder.Property(x => x.StartType).HasMaxLength(32);
        builder.Property(x => x.Account).HasMaxLength(256);
        builder.Property(x => x.Password).HasMaxLength(512);
        builder.Property(x => x.Status).HasMaxLength(32);

        builder.Property(x => x.FailureActionsJson).HasColumnType("nvarchar(max)");
        builder.Property(x => x.DependenciesJson).HasColumnType("nvarchar(max)");
    }
}
