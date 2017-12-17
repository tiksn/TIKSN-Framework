using Nito.AsyncEx;
using System;
using System.Diagnostics;
using System.Management.Automation;
using System.Threading;
using System.Threading.Tasks;

namespace TIKSN.PowerShell
{
    public abstract class CommandBase : PSCmdlet
    {
        private Stopwatch _stopwatch;

        protected IServiceProvider ServiceProvider { get; private set; }
        protected CancellationTokenSource cancellationTokenSource;

        protected CommandBase()
        {
            cancellationTokenSource = new CancellationTokenSource();
        }

        protected override void BeginProcessing()
        {
            cancellationTokenSource = new CancellationTokenSource();

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
            AsyncContext.Run(async () => await ProcessRecordAsync(cancellationTokenSource.Token));
        }

        protected abstract Task ProcessRecordAsync(CancellationToken cancellationToken);

        protected override void StopProcessing()
        {
            cancellationTokenSource.Cancel();
            base.StopProcessing();
        }
    }
}