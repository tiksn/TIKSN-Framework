using System;
using System.Management.Automation;
using System.Threading.Tasks;

namespace PowerShell_Module.Commands
{
	[Cmdlet("Get", "Sample")]
	public class GetSampleCommand : CommandBase
	{
		protected override Task ProcessRecordAsync()
		{
			throw new NotImplementedException();
		}
	}
}
