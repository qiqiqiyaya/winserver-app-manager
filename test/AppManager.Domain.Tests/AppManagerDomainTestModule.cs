using Volo.Abp.Modularity;

namespace AppManager;

[DependsOn(
    typeof(AppManagerDomainModule),
    typeof(AppManagerTestBaseModule)
)]
public class AppManagerDomainTestModule : AbpModule
{

}
