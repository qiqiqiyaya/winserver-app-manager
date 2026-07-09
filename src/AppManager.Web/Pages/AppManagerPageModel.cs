using AppManager.Localization;
using Volo.Abp.AspNetCore.Mvc.UI.RazorPages;

namespace AppManager.Web.Pages;

/* Inherit your PageModel classes from this class.
 */
public abstract class AppManagerPageModel : AbpPageModel
{
    protected AppManagerPageModel()
    {
        LocalizationResourceType = typeof(AppManagerResource);
    }
}
