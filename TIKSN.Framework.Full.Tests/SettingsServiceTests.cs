using Microsoft.Extensions.DependencyInjection;
using TIKSN.Framework.UnitTests.DI;
using TIKSN.Settings;

namespace TIKSN.Framework.UnitTests.Settings
{
    public partial class SettingsServiceTests
    {
        private void SetupDenepdencies()
        {
            Dependencies.ServiceCollection.AddSingleton<ISettingsService, WindowsRegistrySettingsService>();
        }
    }
}