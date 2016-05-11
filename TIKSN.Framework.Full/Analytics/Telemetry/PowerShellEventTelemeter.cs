using System.Management.Automation;
using System.Threading.Tasks;

namespace TIKSN.Analytics.Telemetry
{
	public class PowerShellEventTelemeter : IEventTelemeter
	{
		private readonly Cmdlet cmdlet;

		public PowerShellEventTelemeter(Cmdlet cmdlet)
		{
			this.cmdlet = cmdlet;
		}

		public Task TrackEvent(string name)
		{
			cmdlet.WriteVerbose($"EVENT: {name}");

			return Task.FromResult<object>(null);
		}
	}
}
