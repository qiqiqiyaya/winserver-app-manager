using System.Threading.Tasks;

namespace AppManager.Data;

public interface IAppManagerDbSchemaMigrator
{
    Task MigrateAsync();
}
