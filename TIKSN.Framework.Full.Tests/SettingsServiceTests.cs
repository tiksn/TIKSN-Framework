using Microsoft.Extensions.DependencyInjection;
using TIKSN.Framework.UnitTests.DI;
using TIKSN.Settings;

namespace TIKSN.Framework.UnitTests.Settings
{
    public partial class SettingsServiceTests
    {
        partial void SetupDenepdencies()
        {
            Dependencies.ServiceCollection.AddOptions();
            Dependencies.ServiceCollection.Configure<WindowsRegistrySettingsServiceOptions>(options =>
            {
                options.SubKey = "Software";
            });
            Dependencies.ServiceCollection.AddSingleton<ISettingsService, WindowsRegistrySettingsService>();
        }
    }
}