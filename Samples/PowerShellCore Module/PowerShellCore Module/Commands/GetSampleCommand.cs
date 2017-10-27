﻿using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Management.Automation;
using System.Threading.Tasks;

namespace PowerShellCoreModule.Commands
{
	[Cmdlet("Get", "Sample")]
	public class GetSampleCommand : Command
	{
		protected override async Task ProcessRecordAsync()
		{
			var logger = ServiceProvider.GetRequiredService<ILogger<GetSampleCommand>>();
			logger.LogTrace("some message");
		}
	}
}
