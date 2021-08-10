using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TIKSN.DependencyInjection;
using TIKSN.FileSystem;

namespace TIKSN.Settings.Tests
{
    public partial class SettingsServiceTests
    {
        partial void SetupDenepdencies()
        {
            _ = this.services.AddFrameworkPlatform();
            _ = this.services.AddSingleton<ISettingsService, FileSettingsService>();
            _ = this.services.AddSingleton(new KnownFoldersConfiguration(this.GetType().Assembly,
                KnownFolderVersionConsideration.None));

            var configurationRoot = new ConfigurationBuilder()
                .AddInMemoryCollection()
                .Build();

            configurationRoot["RelativePath"] = "settings.db";

            _ = this.services.ConfigurePartial<FileSettingsServiceOptions, FileSettingsServiceOptionsValidator>(
                configurationRoot);
        }
    }
}
