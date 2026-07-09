using AppManager.Samples;
using Xunit;

namespace AppManager.EntityFrameworkCore.Domains;

[Collection(AppManagerTestConsts.CollectionDefinitionName)]
public class EfCoreSampleDomainTests : SampleDomainTests<AppManagerEntityFrameworkCoreTestModule>
{

}
