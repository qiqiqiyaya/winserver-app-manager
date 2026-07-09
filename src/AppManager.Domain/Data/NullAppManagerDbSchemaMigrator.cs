using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;

namespace AppManager.Data;

/* This is used if database provider does't define
 * IAppManagerDbSchemaMigrator implementation.
 */
public class NullAppManagerDbSchemaMigrator : IAppManagerDbSchemaMigrator, ITransientDependency
{
    public Task MigrateAsync()
    {
        return Task.CompletedTask;
    }
}
