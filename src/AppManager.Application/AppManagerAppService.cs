using System;
using System.Collections.Generic;
using System.Text;
using AppManager.Localization;
using Volo.Abp.Application.Services;

namespace AppManager;

/* Inherit your application services from this class.
 */
public abstract class AppManagerAppService : ApplicationService
{
    protected AppManagerAppService()
    {
        LocalizationResource = typeof(AppManagerResource);
    }
}
