namespace TIKSN.Shell;

public interface IShellCommandContext
{
    public bool ShouldContinue(string query, string caption);

    public bool ShouldContinue(string query, string caption, ref bool yesToAll, ref bool noToAll);

    public bool ShouldProcess(string target, string action);

    public bool ShouldProcess(string target);

    public bool ShouldProcess(string verboseDescription, string verboseWarning, string caption);
}
