using Volo.Abp.Settings;

namespace AppManager.Settings;

public class AppManagerSettingDefinitionProvider : SettingDefinitionProvider
{
    public override void Define(ISettingDefinitionContext context)
    {
        //Define your own settings here. Example:
        //context.Add(new SettingDefinition(AppManagerSettings.MySetting1));
    }
}
