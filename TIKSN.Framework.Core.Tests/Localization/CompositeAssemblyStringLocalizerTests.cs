using System;
using System.Globalization;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Serilog;
using Shouldly;
using TIKSN.DependencyInjection;
using Xunit;
using Xunit.Abstractions;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace TIKSN.Tests.Localization;

public partial class CompositeAssemblyStringLocalizerTests
{
    private readonly IServiceProvider serviceProvider;

    public CompositeAssemblyStringLocalizerTests(ITestOutputHelper testOutputHelper)
    {
        var services = new ServiceCollection();
        _ = services.AddFrameworkCore();
        _ = services.AddLogging(builder =>
        {
            _ = builder.AddDebug();
            var loggger = new LoggerConfiguration()
                .MinimumLevel.Verbose()
                .WriteTo.TestOutput(testOutputHelper, formatProvider: CultureInfo.InvariantCulture)
                .CreateLogger();
            _ = builder.AddSerilog(loggger);
        });
        this.serviceProvider = services.BuildServiceProvider();
    }

    [Fact]
    public void KeyUniqueness()
    {
        var resourceNamesCache = new ResourceNamesCache();
        var testStringLocalizer = new TestStringLocalizer(resourceNamesCache, this.serviceProvider.GetRequiredService<ILogger<TestStringLocalizer>>());
        var allStrings = testStringLocalizer
            .GetAllStrings()
            .GroupBy(item => item.Name.ToLowerInvariant())
            .ToArray();

        var duplicates = allStrings
            .Where(item => item.Count() > 1)
            .ToArray();

        var duplicatesCount = duplicates.Length;
        var logger = this.serviceProvider.GetRequiredService<ILogger<CompositeAssemblyStringLocalizerTests>>();

        foreach (var duplicate in duplicates)
        {
            foreach (var duplicateItem in duplicate)
            {
                LogDuplicateItem(logger, duplicate.Key, duplicateItem.Name, duplicateItem.Value, duplicateItem.SearchedLocation);
            }
        }
        duplicatesCount.ShouldBe(0);
    }

    [LoggerMessage(
        EventId = 21923797,
        Level = LogLevel.Information,
        Message = "Duplicate Localization Key `{LocalizationKey}` Name `{LocalizationName}` Value `{LocalizationValue}` SearchedLocation `{LocalizationSearchedLocation}`")]
    private static partial void LogDuplicateItem(
        ILogger logger, string localizationKey, string localizationName, string localizationValue, string localizationSearchedLocation);
}
