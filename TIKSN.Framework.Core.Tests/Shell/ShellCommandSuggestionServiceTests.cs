using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using Shouldly;
using TIKSN.Shell;
using TIKSN.Shell.AI;
using Xunit;

namespace TIKSN.Tests.Shell;

public class ShellCommandSuggestionServiceTests
{
    [Fact]
    public async Task GivenMalformedResponse_WhenSuggesting_ThenThrowsJsonException()
    {
        var shellCommandEngine = Substitute.For<IShellCommandEngine>();
        _ = shellCommandEngine.GetHelpItems().Returns(
        [
            new ShellCommandHelpItem("Known command", []),
        ]);
        var service = new ShellCommandSuggestionService(CreateChatClient("not JSON"), shellCommandEngine);

        _ = await Should.ThrowAsync<System.Text.Json.JsonException>(() =>
            service.SuggestAsync("Find a command", TestContext.Current.CancellationToken));
    }

    [Fact]
    public async Task GivenNoRegisteredCommands_WhenSuggesting_ThenReturnsNoSuggestionsWithoutCallingAi()
    {
        var shellCommandEngine = Substitute.For<IShellCommandEngine>();
        _ = shellCommandEngine.GetHelpItems().Returns([]);
        var chatClient = Substitute.For<IChatClient>();
        var service = new ShellCommandSuggestionService(chatClient, shellCommandEngine);

        var suggestions = await service.SuggestAsync("Find a command", TestContext.Current.CancellationToken);

        suggestions.ShouldBeEmpty();
        _ = chatClient.DidNotReceive().GetResponseAsync(
            Arg.Any<IEnumerable<ChatMessage>>(),
            Arg.Any<ChatOptions>(),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public void GivenRegisteredCommand_WhenReadingHelpItems_ThenIncludesLocalizedNameAndParameters()
    {
        var localizer = Substitute.For<IStringLocalizer>();
        _ = localizer[Arg.Any<string>()].Returns(callInfo =>
        {
            var key = callInfo.Arg<string>();
            return new LocalizedString(key, key, false);
        });
        var shellCommandEngine = new ShellCommandEngine(
            Substitute.For<IServiceProvider>(),
            NullLogger<ShellCommandEngine>.Instance,
            localizer,
            Substitute.For<IConsoleService>());

        shellCommandEngine.AddType(typeof(CatalogTestCommand));

        var command = shellCommandEngine.GetHelpItems()
            .Single(item => item.CommandName == nameof(CatalogTestCommand));
        command.Parameters.ShouldBe(nameof(CatalogTestCommand.Value));
    }

    [Fact]
    public async Task GivenUnknownParameter_WhenSuggesting_ThenDiscardsSuggestion()
    {
        var shellCommandEngine = Substitute.For<IShellCommandEngine>();
        _ = shellCommandEngine.GetHelpItems().Returns(
        [
            new ShellCommandHelpItem("Known command", []),
        ]);
        var service = new ShellCommandSuggestionService(
            CreateChatClient(
                /*lang=json,strict*/
                """
                [
                  {
                    "commandName": "Known command",
                    "reason": "It might help.",
                    "parameters": { "Unexpected": "value" }
                  }
                ]
                """),
            shellCommandEngine);

        var suggestions = await service.SuggestAsync("Do the task", TestContext.Current.CancellationToken);

        suggestions.ShouldBeEmpty();
    }

    [Fact]
    public async Task GivenValidAndUnknownCommands_WhenSuggesting_ThenReturnsOnlyKnownCommandsAndParameters()
    {
        var shellCommandEngine = Substitute.For<IShellCommandEngine>();
        _ = shellCommandEngine.GetHelpItems().Returns(
        [
            new ShellCommandHelpItem("Convert currency", ["Amount", "Target"]),
        ]);
        var chatClient = CreateChatClient(
            /*lang=json,strict*/
            """
            [
              {
                "commandName": "convert currency",
                "reason": "The request asks for currency conversion.",
                "parameters": {
                  "amount": "25",
                  "Target": "EUR"
                }
              },
              {
                "commandName": "Delete everything",
                "reason": "Unknown command.",
                "parameters": {}
              }
            ]
            """);
        var service = new ShellCommandSuggestionService(chatClient, shellCommandEngine);

        var suggestions = await service.SuggestAsync(
            "Convert 25 dollars to euros",
            TestContext.Current.CancellationToken);

        _ = suggestions.ShouldHaveSingleItem();
        suggestions[0].CommandName.ShouldBe("Convert currency");
        suggestions[0].Parameters["Amount"].ShouldBe("25");
        suggestions[0].Parameters["Target"].ShouldBe("EUR");
    }

    private static IChatClient CreateChatClient(string responseText)
    {
        var chatClient = Substitute.For<IChatClient>();
        _ = chatClient.GetResponseAsync(
                Arg.Any<IEnumerable<ChatMessage>>(),
                Arg.Any<ChatOptions>(),
                Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(
                new ChatResponse(new ChatMessage(ChatRole.Assistant, responseText))));

        return chatClient;
    }

    [ShellCommand(nameof(CatalogTestCommand))]
    public sealed class CatalogTestCommand : IShellCommand
    {
        [ShellCommandParameter(nameof(Value), Mandatory = true)]
        public string Value { get; set; } = string.Empty;

        public Task ExecuteAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }
}
