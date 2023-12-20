using TIKSN.Progress;

namespace TIKSN.Shell;

public class ShellUserConfirmation : IUserConfirmation
{
    private readonly IShellCommandContext shellCommandContext;

    public ShellUserConfirmation(IShellCommandContext shellCommandContext) =>
        this.shellCommandContext = shellCommandContext;

    public bool ShouldContinue(string query, string caption) =>
        this.shellCommandContext.ShouldContinue(query, caption);

    public bool ShouldContinue(string query, string caption, ref bool yesToAll, ref bool noToAll) =>
        this.shellCommandContext.ShouldContinue(query, caption, ref yesToAll, ref noToAll);

    public bool ShouldProcess(string target) => this.shellCommandContext.ShouldProcess(target);

    public bool ShouldProcess(string target, string action) =>
        this.shellCommandContext.ShouldProcess(target, action);

    public bool ShouldProcess(string verboseDescription, string verboseWarning, string caption) =>
        this.shellCommandContext.ShouldProcess(verboseDescription, verboseWarning, caption);
}
