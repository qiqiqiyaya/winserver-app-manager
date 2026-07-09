using Xunit;

namespace AppManager.EntityFrameworkCore;

[CollectionDefinition(AppManagerTestConsts.CollectionDefinitionName)]
public class AppManagerEntityFrameworkCoreCollection : ICollectionFixture<AppManagerEntityFrameworkCoreFixture>
{

}
