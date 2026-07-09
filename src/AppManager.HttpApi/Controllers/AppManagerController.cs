using AppManager.Localization;
using Volo.Abp.AspNetCore.Mvc;

namespace AppManager.Controllers;

/* Inherit your controllers from this class.
 */
public abstract class AppManagerController : AbpControllerBase
{
    protected AppManagerController()
    {
        LocalizationResource = typeof(AppManagerResource);
    }
}
