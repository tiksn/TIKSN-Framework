namespace TIKSN.Progress
{
    public class SilentUserConfirmation : IUserConfirmation
    {
        public bool ShouldContinue(string query, string caption)
        {
            return true;
        }

        public bool ShouldContinue(string query, string caption, ref bool yesToAll, ref bool noToAll)
        {
            if (yesToAll) return true;

            if (noToAll) return false;

            return true;
        }

        public bool ShouldProcess(string target)
        {
            return true;
        }

        public bool ShouldProcess(string target, string action)
        {
            return true;
        }

        public bool ShouldProcess(string verboseDescription, string verboseWarning, string caption)
        {
            return true;
        }
    }
}