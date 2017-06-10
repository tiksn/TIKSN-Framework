using Nito.AsyncEx;
using System;
using System.Diagnostics;
using System.Management.Automation;
using System.Threading.Tasks;

namespace TIKSN.PowerShell
{
	public abstract class CommandBase : PSCmdlet
	{
		private Stopwatch _stopwatch;

		protected IServiceProvider ServiceProvider { get; private set; }

		protected override void BeginProcessing()
		{
			base.BeginProcessing();
			WriteVerbose($"Command started at {DateTime.Now.ToLongTimeString()}.");
			_stopwatch = Stopwatch.StartNew();

			ServiceProvider = CreateServiceProvider();

			//if (!string.IsNullOrEmpty(this.Language))
			//{
			//    var contentCulture = new CultureInfo(this.Language);

			//    Thread.CurrentThread.CurrentCulture = contentCulture;
			//    Thread.CurrentThread.CurrentUICulture = contentCulture;
			//}
		}

		protected abstract IServiceProvider CreateServiceProvider();

		protected override void EndProcessing()
		{
			_stopwatch.Stop();
			WriteVerbose(
				$"Command finished at {DateTime.Now.ToLongTimeString()}. It took {_stopwatch.Elapsed} to complete.");
			base.EndProcessing();
		}

		protected sealed override void ProcessRecord()
		{
			AsyncContext.Run(async () => await ProcessRecordAsync());
		}

		protected abstract Task ProcessRecordAsync();
	}
}