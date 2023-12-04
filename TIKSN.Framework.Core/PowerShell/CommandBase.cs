using System.Management.Automation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Nito.AsyncEx;

namespace TIKSN.PowerShell;

public abstract class CommandBase : PSCmdlet, IDisposable
{
    protected CancellationTokenSource cancellationTokenSource;
    private IServiceScope serviceScope;

    protected CommandBase() => this.cancellationTokenSource = new CancellationTokenSource();

    protected IServiceProvider ServiceProvider => this.serviceScope.ServiceProvider;

    protected override void BeginProcessing()
    {
        this.cancellationTokenSource = new CancellationTokenSource();

        base.BeginProcessing();

        var topServiceProvider = this.GetServiceProvider();
        this.serviceScope = topServiceProvider.CreateScope();
        this.ServiceProvider.GetRequiredService<ICurrentCommandStore>().SetCurrentCommand(this);
        this.ConfigureLogger(this.ServiceProvider.GetRequiredService<ILoggerFactory>());
    }

    protected virtual void ConfigureLogger(ILoggerFactory loggerFactory) =>
        loggerFactory.AddPowerShell(this.ServiceProvider);

    protected abstract IServiceProvider GetServiceProvider();

    protected sealed override void ProcessRecord() => AsyncContext.Run(async () =>
        await this.ProcessRecordAsync(this.cancellationTokenSource.Token).ConfigureAwait(false));

    protected abstract Task ProcessRecordAsync(CancellationToken cancellationToken);

    protected override void StopProcessing()
    {
        this.cancellationTokenSource.Cancel();
        base.StopProcessing();
        this.serviceScope.Dispose();
    }

    public void Dispose() => throw new NotImplementedException();
}
