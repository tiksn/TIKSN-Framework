using TIKSN.Progress;

namespace TIKSN.PowerShell;

public class PowerShellUserConfirmation : IUserConfirmation
{
    private readonly ICurrentCommandProvider currentCommandProvider;

    public PowerShellUserConfirmation(ICurrentCommandProvider currentCommandProvider) =>
        this.currentCommandProvider = currentCommandProvider
        ?? throw new ArgumentNullException(nameof(currentCommandProvider));

    public bool ShouldContinue(string query, string caption) =>
        this.currentCommandProvider.GetCurrentCommand().ShouldContinue(query, caption);

    public bool ShouldContinue(string query, string caption, ref bool yesToAll, ref bool noToAll) => this
        .currentCommandProvider.GetCurrentCommand().ShouldContinue(query, caption, ref yesToAll, ref noToAll);

    public bool ShouldProcess(string target) =>
        this.currentCommandProvider.GetCurrentCommand().ShouldProcess(target);

    public bool ShouldProcess(string target, string action) =>
        this.currentCommandProvider.GetCurrentCommand().ShouldProcess(target, action);

    public bool ShouldProcess(string verboseDescription, string verboseWarning, string caption) => this
        .currentCommandProvider.GetCurrentCommand().ShouldProcess(verboseDescription, verboseDescription, caption);
}
