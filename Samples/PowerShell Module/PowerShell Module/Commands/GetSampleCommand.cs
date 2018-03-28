using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Management.Automation;
using System.Threading;
using System.Threading.Tasks;

namespace PowerShell_Module.Commands
{
	[Cmdlet("Get", "Sample")]
	public class GetSampleCommand : Command
	{
		protected override async Task ProcessRecordAsync(CancellationToken cancellationToken)
		{
			var logger = ServiceProvider.GetRequiredService<ILogger<GetSampleCommand>>();
			logger.LogTrace("some message");
		}
	}
}
