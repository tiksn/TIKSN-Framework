namespace TIKSN.Progress;

public class SilentUserConfirmation : IUserConfirmation
{
    public bool ShouldContinue(string query, string caption) => true;

    public bool ShouldContinue(string query, string caption, ref bool yesToAll, ref bool noToAll)
    {
        if (yesToAll)
        {
            return true;
        }

        return !noToAll;
    }

    public bool ShouldProcess(string target) => true;

    public bool ShouldProcess(string target, string action) => true;

    public bool ShouldProcess(string verboseDescription, string verboseWarning, string caption) => true;
}
