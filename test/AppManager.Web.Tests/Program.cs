using Microsoft.AspNetCore.Builder;
using AppManager;
using Volo.Abp.AspNetCore.TestBase;

var builder = WebApplication.CreateBuilder();
await builder.RunAbpModuleAsync<AppManagerWebTestModule>();

public partial class Program
{
}
