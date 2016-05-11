using System.Management.Automation;

namespace TIKSN.Progress
{
	public class PowerShellUserConfirmation : IUserConfirmation
	{
		private readonly Cmdlet cmdlet;

		public PowerShellUserConfirmation(Cmdlet cmdlet)
		{
			this.cmdlet = cmdlet;
		}

		public bool ShouldProcess(string target)
		{
			return this.cmdlet.ShouldProcess(target);
		}

		public bool ShouldProcess(string target, string action)
		{
			return cmdlet.ShouldProcess(target, action);
		}

		public bool ShouldProcess(string verboseDescription, string verboseWarning, string caption)
		{
			return cmdlet.ShouldProcess(verboseDescription, verboseDescription, caption);
		}
	}
}
