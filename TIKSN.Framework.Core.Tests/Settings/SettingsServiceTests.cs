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
            this.services.AddFrameworkPlatform();
            this.services.AddSingleton<ISettingsService, FileSettingsService>();
            this.services.AddSingleton(new KnownFoldersConfiguration(this.GetType().Assembly,
                KnownFolderVersionConsideration.None));

            var configurationRoot = new ConfigurationBuilder()
                .AddInMemoryCollection()
                .Build();

            configurationRoot["RelativePath"] = "settings.db";

            this.services.ConfigurePartial<FileSettingsServiceOptions, FileSettingsServiceOptionsValidator>(
                configurationRoot);
        }
    }
}
