using TIKSN.Progress;

namespace TIKSN.Shell
{
	public class ShellUserConfirmation : IUserConfirmation
	{
		private readonly IShellCommandContext _shellCommandContext;

		public ShellUserConfirmation(IShellCommandContext shellCommandContext)
		{
			_shellCommandContext = shellCommandContext;
		}

		public bool ShouldContinue(string query, string caption)
		{
			return _shellCommandContext.ShouldContinue(query, caption);
		}

		public bool ShouldContinue(string query, string caption, ref bool yesToAll, ref bool noToAll)
		{
			return _shellCommandContext.ShouldContinue(query, caption, ref yesToAll, ref noToAll);
		}

		public bool ShouldProcess(string target)
		{
			return _shellCommandContext.ShouldProcess(target);
		}

		public bool ShouldProcess(string target, string action)
		{
			return _shellCommandContext.ShouldProcess(target, action);
		}

		public bool ShouldProcess(string verboseDescription, string verboseWarning, string caption)
		{
			return _shellCommandContext.ShouldProcess(verboseDescription, verboseWarning, caption);
		}
	}
}