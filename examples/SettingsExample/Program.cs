using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Spectre.Console;
using TIKSN.DependencyInjection;
using TIKSN.FileSystem;
using TIKSN.Settings;

var services = new ServiceCollection();
services.AddFrameworkCore();

services.AddLogging(builder =>
{
    builder.SetMinimumLevel(LogLevel.Warning);
    builder.AddConsole();
});

var configurationBuilder = new ConfigurationBuilder();
configurationBuilder.AddInMemoryCollection(new Dictionary<string, string?>
{
    { "Settings:RelativePath", "settings.json" }
});
var configuration = configurationBuilder.Build();

services
    .AddOptions<FileSettingsServiceOptions>()
    .Bind(configuration.GetSection("Settings"))
    .ValidateOnStart();

services.AddSingleton(new KnownFoldersConfiguration(System.Reflection.Assembly.GetExecutingAssembly(), KnownFolderVersionConsideration.None));

// Assuming FileSettingsService implements ISettingsService
services.AddSingleton<ISettingsService, FileSettingsService>();

await using var serviceProvider = services.BuildServiceProvider();

var settingsService = serviceProvider.GetRequiredService<ISettingsService>();

AnsiConsole.MarkupLine("[bold cyan]TIKSN Settings Service Example[/]");
AnsiConsole.MarkupLine("[grey]--------------------------------------[/]");

// 1. Set Local Setting
AnsiConsole.MarkupLine("[yellow]Setting 'Theme' to 'Dark'...[/]");
settingsService.SetLocalSetting("Theme", "Dark");

// 2. Set Roaming Setting
AnsiConsole.MarkupLine("[yellow]Setting roaming preference 'Language' to 'en-US'...[/]");
settingsService.SetRoamingSetting("Language", "en-US");

// 3. Retrieve with default
var theme = settingsService.GetLocalSetting("Theme", "Light");
AnsiConsole.MarkupLine($"[fuchsia]Retrieved Local 'Theme':[/] {theme}");

// 4. Retrieve missing setting with default fallback
var timezone = settingsService.GetLocalSetting("Timezone", "UTC");
AnsiConsole.MarkupLine($"[fuchsia]Retrieved Missing 'Timezone' with fallback:[/] {timezone}");

// 5. Retrieve using LanguageExt Option
var langOpt = settingsService.GetRoamingSetting<string>("Language");
langOpt.Match(
    lang => AnsiConsole.MarkupLine($"[fuchsia]Retrieved Roaming 'Language':[/] {lang}"),
    () => AnsiConsole.MarkupLine("[bold red]Language setting not found![/]")
);

// 6. List all settings
var localSettingsKeys = settingsService.ListLocalSetting();
AnsiConsole.MarkupLine($"[yellow]All Local Keys:[/] {string.Join(", ", localSettingsKeys)}");

var roamingSettingsKeys = settingsService.ListRoamingSetting();
AnsiConsole.MarkupLine($"[yellow]All Roaming Keys:[/] {string.Join(", ", roamingSettingsKeys)}");

// 7. Clean up
AnsiConsole.MarkupLine("[yellow]Removing 'Theme' setting...[/]");
settingsService.RemoveLocalSetting("Theme");

var updatedKeys = settingsService.ListLocalSetting();
AnsiConsole.MarkupLine($"[yellow]Local Keys After Removal:[/] {string.Join(", ", updatedKeys)}");
