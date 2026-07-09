using Volo.Abp.Modularity;

namespace AppManager;

/* Inherit from this class for your domain layer tests. */
public abstract class AppManagerDomainTestBase<TStartupModule> : AppManagerTestBase<TStartupModule>
    where TStartupModule : IAbpModule
{

}
