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
            PlatformDependencyRegistration.Register(services);
            services.AddSingleton<ISettingsService, FileSettingsService>();
            services.AddSingleton(new KnownFoldersConfiguration(GetType().Assembly, KnownFolderVersionConsideration.None));

            var configurationRoot = new ConfigurationBuilder()
                .AddInMemoryCollection()
                .Build();

            configurationRoot["RelativePath"] = "settings.db";

            services.ConfigurePartial<FileSettingsServiceOptions, FileSettingsServiceOptionsValidator>(configurationRoot);
        }
    }
}