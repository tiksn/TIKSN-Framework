using System;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Shouldly;
using TIKSN.DependencyInjection;
using Xunit;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace TIKSN.Tests.Localization;

public partial class CompositeAssemblyStringLocalizerTests
{
    private readonly IServiceProvider serviceProvider;

    public CompositeAssemblyStringLocalizerTests(ITestOutputHelper testOutputHelper)
    {
        var services = new ServiceCollection();
        _ = services.AddFrameworkCore();
        _ = services.AddLogging(builder => _ = builder.AddDebug());
        this.serviceProvider = services.BuildServiceProvider();
    }

    [Fact]
    public void KeyUniqueness()
    {
        var resourceNamesCache = new ResourceNamesCache();
        var testStringLocalizer = new TestStringLocalizer(resourceNamesCache,
            this.serviceProvider.GetRequiredService<ILogger<TestStringLocalizer>>());
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
                LogDuplicateItem(logger, duplicate.Key, duplicateItem.Name, duplicateItem.Value,
                    duplicateItem.SearchedLocation);
            }
        }

        duplicatesCount.ShouldBe(0);
    }

    [Fact]
    public void KeysGenerated()
    {
        var resourceNamesCache = new ResourceNamesCache();
        var testStringLocalizer = new TestStringLocalizer(resourceNamesCache,
            this.serviceProvider.GetRequiredService<ILogger<TestStringLocalizer>>());
        var expectedKeys = testStringLocalizer
            .GetAllStrings()
            .Select(item => item.Name)
            .OrderBy(x => x, StringComparer.Ordinal)
            .ToArray();

        var actualKeys = typeof(TIKSN.Localization.LocalizationKeys)
            .GetProperties(BindingFlags.Public | BindingFlags.Static)
            .Select(property => (string?)property.GetValue(null))
            .OrderBy(x => x, StringComparer.Ordinal)
            .ToArray();

        actualKeys.ShouldBe(expectedKeys);
    }

    [LoggerMessage(
        EventId = 21923797,
        Level = LogLevel.Information,
        Message =
            "Duplicate Localization Key `{LocalizationKey}` Name `{LocalizationName}` " +
            "Value `{LocalizationValue}` SearchedLocation `{LocalizationSearchedLocation}`")]
    private static partial void LogDuplicateItem(
        ILogger logger, string localizationKey, string localizationName, string localizationValue,
        string? localizationSearchedLocation);
}
