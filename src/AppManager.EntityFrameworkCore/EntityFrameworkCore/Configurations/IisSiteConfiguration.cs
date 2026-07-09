using AppManager.IisSite;
using IisSiteEntity = AppManager.IisSite.IisSite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Volo.Abp.EntityFrameworkCore.Modeling;

namespace AppManager.EntityFrameworkCore.Configurations;

public class IisSiteConfiguration : IEntityTypeConfiguration<IisSiteEntity>
{
    public void Configure(EntityTypeBuilder<IisSiteEntity> builder)
    {
        builder.ToTable(AppManagerConsts.DbTablePrefix + "IisSites", AppManagerConsts.DbSchema);
        builder.ConfigureByConvention();

        builder.Property(x => x.SiteName).IsRequired().HasMaxLength(256);
        builder.HasIndex(x => x.SiteName).IsUnique();

        builder.Property(x => x.PhysicalPath).IsRequired().HasMaxLength(1024);
        builder.Property(x => x.Status).HasMaxLength(32);
        builder.Property(x => x.AppPoolName).HasMaxLength(256);

        builder.Property(x => x.BindingsJson).HasColumnType("nvarchar(max)");
        builder.Property(x => x.AppPoolConfigJson).HasColumnType("nvarchar(max)");
        builder.Property(x => x.SubApplicationsJson).HasColumnType("nvarchar(max)");
        builder.Property(x => x.VirtualDirectoriesJson).HasColumnType("nvarchar(max)");
        builder.Property(x => x.NtfsPermissionsJson).HasColumnType("nvarchar(max)");
    }
}
