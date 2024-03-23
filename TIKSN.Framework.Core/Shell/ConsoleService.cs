using System.Reactive.Disposables;
using System.Reflection;
using System.Security;
using Microsoft.Extensions.Localization;
using Spectre.Console;
using TIKSN.Localization;

namespace TIKSN.Shell;

public class ConsoleService : IConsoleService
{
    private readonly IAnsiConsole ansiConsole;
    private readonly IStringLocalizer stringLocalizer;

    public ConsoleService(IStringLocalizer stringLocalizer, IAnsiConsole ansiConsole)
    {
        this.stringLocalizer = stringLocalizer ?? throw new ArgumentNullException(nameof(stringLocalizer));
        this.ansiConsole = ansiConsole ?? throw new ArgumentNullException(nameof(ansiConsole));
    }

    public string ReadLine(string promptMessage, ConsoleColor promptForegroundColor)
    {
        var textPrompt = this.WriteTextPrompt(promptMessage, promptForegroundColor);
        return this.ansiConsole.Prompt(textPrompt);
    }

    public SecureString ReadPasswordLine(string promptMessage, ConsoleColor promptForegroundColor)
    {
        var textPrompt =
            this.WriteTextPrompt(promptMessage, promptForegroundColor)
            .Secret(mask: null);

        var secret = this.ansiConsole.Prompt(textPrompt);
        var pwd = new SecureString();
        secret.ForEach(pwd.AppendChar);

        return pwd;
    }

    public IDisposable RegisterCancellation(CancellationTokenSource cancellationTokenSource)
    {
        void consoleCancelEventHandler(object sender, ConsoleCancelEventArgs e)
        {
            cancellationTokenSource.Cancel();

            e.Cancel = true;
        }

        Console.CancelKeyPress += consoleCancelEventHandler;

        return Disposable.Create(() => Console.CancelKeyPress -= consoleCancelEventHandler);
    }

    public int UserPrompt(string message, params string[] options)
    {
        if (options is null || options.Length == 0)
        {
            throw new ArgumentException("User prompt must contain at least one options",
                nameof(options));
        }

        while (true)
        {
            this.ConsoleWrite(
                $"{message} [{string.Join('/', options)}]{this.stringLocalizer.GetRequiredString(LocalizationKeys.Key444677337)}");

            var answer = Console.ReadLine();

            for (var i = 0; i < options.Length; i++)
            {
                if (string.Equals(options[i], answer, StringComparison.OrdinalIgnoreCase))
                {
                    return i;
                }
            }
        }
    }

    public void WriteError(string errorMessage) => this.ConsoleWriteLine(errorMessage, ConsoleColor.Red);

    public void WriteObject<T>(T value)
    {
        var tableValues = new List<T> { value };
        this.WriteObjects(tableValues, title: null);
    }

    public void WriteObjects<T>(IEnumerable<T> values)
    {
        ArgumentNullException.ThrowIfNull(values);

        this.WriteObjects(values, title: null);
    }

    private void ConsoleWrite(string message) => this.ansiConsole.Write(message);

    private void ConsoleWrite(string message, ConsoleColor foreground)
        => this.ansiConsole.Write(new Text(message.EscapeMarkup(), new Style(foreground)));

    private void ConsoleWriteLine(string message, ConsoleColor foreground)
    {
        this.ConsoleWrite(message, foreground);
        this.ansiConsole.WriteLine();
    }

    private void WriteObjects<T>(IEnumerable<T> items, string title)
    {
        var itemType = typeof(T);
        var itemProperties = itemType.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.GetProperty);

        var table = new Table();
        if (!string.IsNullOrWhiteSpace(title))
        {
            table.Title = new TableTitle(title);
        }

        foreach (var elementProperty in itemProperties)
        {
            _ = table.AddColumn(new TableColumn(elementProperty.Name));
        }

        foreach (var item in items)
        {
            var row = itemProperties
                .Select(p => p.GetValue(item))
                .Select(x => new Text(x.ToString()))
                .ToArray();

            _ = table.AddRow(row);
        }

        this.ansiConsole.Write(table);
        this.ansiConsole.WriteLine();
    }

    private TextPrompt<string> WriteTextPrompt(string promptMessage, ConsoleColor promptForegroundColor)
    {
        var promptIndicator = this.stringLocalizer.GetRequiredString(LocalizationKeys.Key444677337);

        this.ansiConsole.Write(new Text(promptMessage, new Style(promptForegroundColor)));
        this.ansiConsole.Write(new Text(promptIndicator, new Style(promptForegroundColor)));

        return new TextPrompt<string>(string.Empty)
            .AllowEmpty();
    }
}
