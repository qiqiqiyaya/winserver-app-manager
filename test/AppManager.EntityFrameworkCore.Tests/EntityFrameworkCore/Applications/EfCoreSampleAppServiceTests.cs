using AppManager.Samples;
using Xunit;

namespace AppManager.EntityFrameworkCore.Applications;

[Collection(AppManagerTestConsts.CollectionDefinitionName)]
public class EfCoreSampleAppServiceTests : SampleAppServiceTests<AppManagerEntityFrameworkCoreTestModule>
{

}
