using System.Globalization;
using Microsoft.Extensions.Localization;
using TIKSN.Localization;

namespace TIKSN.Shell;

public class ShellCommandContext : IShellCommandContext, IShellCommandContextStore
{
    private readonly IConsoleService consoleService;
    private readonly IStringLocalizer stringLocalizer;

    private string? commandName;
    private bool noToAll;
    private bool yesToAll;

    public ShellCommandContext(IStringLocalizer stringLocalizer, IConsoleService consoleService)
    {
        this.stringLocalizer = stringLocalizer;
        this.consoleService = consoleService;
    }

    public void SetCommandName(string commandName) => this.commandName = commandName;

    public bool ShouldContinue(string query, string caption)
    {
        var message = caption + Environment.NewLine + query;

        var answer = this.consoleService.UserPrompt(message,
            this.stringLocalizer.GetRequiredString(LocalizationKeys.Key592470584),
            this.stringLocalizer.GetRequiredString(LocalizationKeys.Key132999259),
            this.stringLocalizer.GetRequiredString(LocalizationKeys.Key777755530));

        return answer switch
        {
            0 => true,
            1 => false,
            2 => throw new ShellCommandSuspendedException(),
            _ => throw new NotSupportedException(),
        };
    }

    public bool ShouldContinue(string query, string caption, ref bool yesToAll, ref bool noToAll)
    {
        if (yesToAll)
        {
            return true;
        }

        if (noToAll)
        {
            return false;
        }

        var message = caption + Environment.NewLine + query;

        var answer = this.consoleService.UserPrompt(message,
            this.stringLocalizer.GetRequiredString(LocalizationKeys.Key592470584),
            this.stringLocalizer.GetRequiredString(LocalizationKeys.Key268507527),
            this.stringLocalizer.GetRequiredString(LocalizationKeys.Key132999259),
            this.stringLocalizer.GetRequiredString(LocalizationKeys.Key191067042),
            this.stringLocalizer.GetRequiredString(LocalizationKeys.Key777755530));

        switch (answer)
        {
            case 0:
                return true;

            case 1:
                yesToAll = true;
                return true;

            case 2:
                return false;

            case 3:
                noToAll = true;
                return false;

            case 4:
                throw new ShellCommandSuspendedException();

            default:
                throw new NotSupportedException();
        }
    }

    public bool ShouldProcess(string target, string action) =>
        this.ShouldContinue(this.stringLocalizer.GetRequiredString(LocalizationKeys.Key439104548),
            string.Format(CultureInfo.InvariantCulture, this.stringLocalizer.GetRequiredString(LocalizationKeys.Key284914810), action, target),
            ref this.yesToAll, ref this.noToAll);

    public bool ShouldProcess(string target)
    {
        if (this.commandName is null)
        {
            throw new InvalidOperationException("Command Name cannot be NULL.");
        }

        return this.ShouldProcess(target, this.commandName);
    }

    public bool ShouldProcess(string verboseDescription, string verboseWarning, string caption) =>
        this.ShouldContinue(verboseWarning, caption, ref this.yesToAll, ref this.noToAll);
}
