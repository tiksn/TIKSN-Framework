using TIKSN.Progress;

namespace TIKSN.Shell;

public class ShellUserConfirmation : IUserConfirmation
{
    private readonly IShellCommandContext _shellCommandContext;

    public ShellUserConfirmation(IShellCommandContext shellCommandContext) =>
        this._shellCommandContext = shellCommandContext;

    public bool ShouldContinue(string query, string caption) =>
        this._shellCommandContext.ShouldContinue(query, caption);

    public bool ShouldContinue(string query, string caption, ref bool yesToAll, ref bool noToAll) =>
        this._shellCommandContext.ShouldContinue(query, caption, ref yesToAll, ref noToAll);

    public bool ShouldProcess(string target) => this._shellCommandContext.ShouldProcess(target);

    public bool ShouldProcess(string target, string action) =>
        this._shellCommandContext.ShouldProcess(target, action);

    public bool ShouldProcess(string verboseDescription, string verboseWarning, string caption) =>
        this._shellCommandContext.ShouldProcess(verboseDescription, verboseWarning, caption);
}
