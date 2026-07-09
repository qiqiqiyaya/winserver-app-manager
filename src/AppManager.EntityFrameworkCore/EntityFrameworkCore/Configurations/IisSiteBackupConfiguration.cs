using AppManager.Backups;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Volo.Abp.EntityFrameworkCore.Modeling;

namespace AppManager.EntityFrameworkCore.Configurations;

public class IisSiteBackupConfiguration : IEntityTypeConfiguration<IisSiteBackup>
{
    public void Configure(EntityTypeBuilder<IisSiteBackup> builder)
    {
        builder.ToTable(AppManagerConsts.DbTablePrefix + "IisSiteBackups", AppManagerConsts.DbSchema);
        builder.ConfigureByConvention();

        builder.Property(x => x.SiteName).IsRequired().HasMaxLength(256);
        builder.HasIndex(x => x.SiteName);
        builder.HasIndex(x => x.CreatedAt);

        builder.Property(x => x.BackupData).IsRequired().HasColumnType("nvarchar(max)");
        builder.Property(x => x.Description).HasMaxLength(512);
        builder.Property(x => x.CreatedBy).HasMaxLength(256);
    }
}
