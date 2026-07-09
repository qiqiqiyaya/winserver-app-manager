using System.Threading.Tasks;
using Shouldly;
using Xunit;

namespace AppManager.Pages;

public class Index_Tests : AppManagerWebTestBase
{
    [Fact]
    public async Task Welcome_Page()
    {
        var response = await GetResponseAsStringAsync("/");
        response.ShouldNotBeNull();
    }
}
