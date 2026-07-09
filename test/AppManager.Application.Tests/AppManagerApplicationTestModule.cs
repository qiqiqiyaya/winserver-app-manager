using Volo.Abp.Modularity;

namespace AppManager;

[DependsOn(
    typeof(AppManagerApplicationModule),
    typeof(AppManagerDomainTestModule)
)]
public class AppManagerApplicationTestModule : AbpModule
{

}
