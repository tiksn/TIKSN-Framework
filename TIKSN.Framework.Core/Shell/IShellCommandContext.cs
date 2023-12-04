namespace TIKSN.Shell;

public interface IShellCommandContext
{
    bool ShouldContinue(string query, string caption);

    bool ShouldContinue(string query, string caption, ref bool yesToAll, ref bool noToAll);

    bool ShouldProcess(string target, string action);

    bool ShouldProcess(string target);

    bool ShouldProcess(string verboseDescription, string verboseWarning, string caption);
}
