using AppManager.EntityFrameworkCore;
using Volo.Abp.Autofac;
using Volo.Abp.Modularity;

namespace AppManager.DbMigrator;

[DependsOn(
    typeof(AbpAutofacModule),
    typeof(AppManagerEntityFrameworkCoreModule),
    typeof(AppManagerApplicationContractsModule)
    )]
public class AppManagerDbMigratorModule : AbpModule
{
}
