using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Nito.AsyncEx;
using System;
using System.Management.Automation;
using System.Threading;
using System.Threading.Tasks;

namespace TIKSN.PowerShell
{
    public abstract class CommandBase : PSCmdlet
    {
        protected CancellationTokenSource cancellationTokenSource;
        private IServiceScope serviceScope;

        protected CommandBase()
        {
            cancellationTokenSource = new CancellationTokenSource();
        }

        protected IServiceProvider ServiceProvider => serviceScope.ServiceProvider;

        protected override void BeginProcessing()
        {
            cancellationTokenSource = new CancellationTokenSource();

            base.BeginProcessing();

            var topServiceProvider = GetServiceProvider();
            serviceScope = topServiceProvider.CreateScope();
            ServiceProvider.GetRequiredService<ICurrentCommandStore>().SetCurrentCommand(this);
            ConfigureLogger(ServiceProvider.GetRequiredService<ILoggerFactory>());
        }

        protected virtual void ConfigureLogger(ILoggerFactory loggerFactory)
        {
            loggerFactory.AddPowerShell(ServiceProvider);
        }

        protected abstract IServiceProvider GetServiceProvider();

        protected sealed override void ProcessRecord()
        {
            AsyncContext.Run(async () => await ProcessRecordAsync(cancellationTokenSource.Token));
        }

        protected abstract Task ProcessRecordAsync(CancellationToken cancellationToken);

        protected override void StopProcessing()
        {
            cancellationTokenSource.Cancel();
            base.StopProcessing();
            serviceScope.Dispose();
        }
    }
}