using System;
using System.Globalization;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Serilog;
using Spectre.Console;
using Spectre.Console.Testing;
using TIKSN.DependencyInjection;
using TIKSN.Shell;
using TIKSN.Tests.Localization;
using Xunit;
using Xunit.Abstractions;
using static LanguageExt.Prelude;

namespace TIKSN.Tests.Shell;

public class ConsoleServiceTests
{
    private readonly ITestOutputHelper testOutputHelper;

    public ConsoleServiceTests(ITestOutputHelper testOutputHelper)
        => this.testOutputHelper = testOutputHelper ?? throw new System.ArgumentNullException(nameof(testOutputHelper));

    [Fact]
    public async Task GivenDependencies_WhenHelpCommandAndExitCalled_ThenOutputShouldMatch()
    {
        // Arrange
        var services = new ServiceCollection();
        using var testConsole = new TestConsole()
            .EmitAnsiSequences();
        _ = services.AddSingleton<IAnsiConsole>(testConsole);
        _ = services.AddSingleton<IStringLocalizer, TestStringLocalizer>();
        _ = services.AddFrameworkCore();
        _ = services.AddLogging(builder =>
        {
            _ = builder.AddDebug();
            var loggger = new LoggerConfiguration()
                .MinimumLevel.Verbose()
                .WriteTo.TestOutput(this.testOutputHelper, formatProvider: CultureInfo.InvariantCulture)
                .CreateLogger();
            _ = builder.AddSerilog(loggger);
        });
        var serviceProvider = services.BuildServiceProvider();
        var shellCommandEngine = serviceProvider.GetRequiredService<IShellCommandEngine>();

        // Act
        testConsole.Input.PushTextWithEnter("help");
        testConsole.Input.PushTextWithEnter("exit");
        await shellCommandEngine.RunAsync();

        var actualOutputLines = testConsole.Output.Split(Environment.NewLine);
        var expectedOutputLines = Seq(
            "\u001b[38;5;10mCommand\u001b[0m\u001b[38;5;10m:\u001b[0m help",
            "┌─────────────┬────────────┐",
            "│ CommandName │ Parameters │",
            "├─────────────┼────────────┤",
            "│ Exit        │            │",
            "│ Help        │            │",
            "└─────────────┴────────────┘",
            "",
            "\u001b[38;5;10mCommand\u001b[0m\u001b[38;5;10m:\u001b[0m exit",
            "");

        _ = actualOutputLines.Should().BeEquivalentTo(expectedOutputLines);
    }
}
