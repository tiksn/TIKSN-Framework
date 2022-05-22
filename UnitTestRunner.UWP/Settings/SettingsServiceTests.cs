using Microsoft.Extensions.DependencyInjection;

namespace TIKSN.Settings.Tests
{
    public partial class SettingsServiceTests
    {
        partial void SetupDenepdencies()
        {
            services.AddSingleton<ISettingsService, SettingsService>();
        }
    }
}
