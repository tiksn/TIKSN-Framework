using Microsoft.Extensions.DependencyInjection;

namespace TIKSN.Settings.Tests
{
    public partial class SettingsServiceTests
    {
        partial void SetupDenepdencies()
        {
            services.AddOptions();
            services.Configure<WindowsRegistrySettingsServiceOptions>(options =>
            {
                options.SubKey = "Software";
            });
            services.AddSingleton<ISettingsService, WindowsRegistrySettingsService>();
        }
    }
}