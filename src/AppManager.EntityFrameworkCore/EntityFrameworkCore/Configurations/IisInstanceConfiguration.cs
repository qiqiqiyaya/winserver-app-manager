using AppManager.IisSite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Volo.Abp.EntityFrameworkCore.Modeling;

namespace AppManager.EntityFrameworkCore.Configurations;

public class IisInstanceConfiguration : IEntityTypeConfiguration<IisInstance>
{
    public void Configure(EntityTypeBuilder<IisInstance> builder)
    {
        builder.ToTable(AppManagerConsts.DbTablePrefix + "IisInstances", AppManagerConsts.DbSchema);
        builder.ConfigureByConvention();

        builder.Property(x => x.Name).IsRequired().HasMaxLength(256);
        builder.Property(x => x.ConfigPath).IsRequired().HasMaxLength(1024);
        builder.HasIndex(x => x.ConfigPath).IsUnique();
        builder.Property(x => x.Status).HasMaxLength(32);
    }
}
