using AppManager.Backups;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Volo.Abp.EntityFrameworkCore.Modeling;

namespace AppManager.EntityFrameworkCore.Configurations;

public class WindowsServiceBackupConfiguration : IEntityTypeConfiguration<WindowsServiceBackup>
{
    public void Configure(EntityTypeBuilder<WindowsServiceBackup> builder)
    {
        builder.ToTable(AppManagerConsts.DbTablePrefix + "WindowsServiceBackups", AppManagerConsts.DbSchema);
        builder.ConfigureByConvention();

        builder.Property(x => x.ServiceName).IsRequired().HasMaxLength(256);
        builder.HasIndex(x => x.ServiceName);
        builder.HasIndex(x => x.CreatedAt);

        builder.Property(x => x.DisplayName).HasMaxLength(256);
        builder.Property(x => x.BackupData).IsRequired().HasColumnType("nvarchar(max)");
        builder.Property(x => x.Description).HasMaxLength(512);
        builder.Property(x => x.CreatedBy).HasMaxLength(256);
    }
}
