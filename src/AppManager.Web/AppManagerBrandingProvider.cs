using Microsoft.Extensions.Localization;
using AppManager.Localization;
using Volo.Abp.Ui.Branding;
using Volo.Abp.DependencyInjection;

namespace AppManager.Web;

[Dependency(ReplaceServices = true)]
public class AppManagerBrandingProvider : DefaultBrandingProvider
{
    private IStringLocalizer<AppManagerResource> _localizer;

    public AppManagerBrandingProvider(IStringLocalizer<AppManagerResource> localizer)
    {
        _localizer = localizer;
    }

    public override string AppName => _localizer["AppName"];
}
