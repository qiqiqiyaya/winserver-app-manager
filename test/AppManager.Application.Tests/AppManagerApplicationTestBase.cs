using Volo.Abp.Modularity;

namespace AppManager;

public abstract class AppManagerApplicationTestBase<TStartupModule> : AppManagerTestBase<TStartupModule>
    where TStartupModule : IAbpModule
{

}
