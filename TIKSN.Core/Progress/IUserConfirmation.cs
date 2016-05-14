namespace TIKSN.Progress
{
	public interface IUserConfirmation
	{
		bool ShouldProcess(string target);

		bool ShouldProcess(string target, string action);

		bool ShouldProcess(string verboseDescription, string verboseWarning, string caption);
	}
}