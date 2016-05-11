namespace TIKSN.Progress
{
	public class SilentUserConfirmation : IUserConfirmation
	{
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
