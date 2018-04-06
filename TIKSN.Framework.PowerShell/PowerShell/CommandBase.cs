using Nito.AsyncEx;
using System;
using System.Management.Automation;
using System.Threading;
using System.Threading.Tasks;

namespace TIKSN.PowerShell
{
    public abstract class CommandBase : PSCmdlet
    {
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

            ServiceProvider = CreateServiceProvider();
        }

        protected abstract IServiceProvider CreateServiceProvider();

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