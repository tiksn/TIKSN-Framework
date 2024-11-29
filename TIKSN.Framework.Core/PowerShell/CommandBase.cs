using System.Management.Automation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Nito.AsyncEx;

namespace TIKSN.PowerShell;

public abstract class CommandBase : PSCmdlet, IDisposable
{
    private CancellationTokenSource cancellationTokenSource;
    private bool disposedValue;
    private IServiceScope? serviceScope;

    protected CommandBase() => this.cancellationTokenSource = new CancellationTokenSource();

    protected IServiceProvider Services => this.serviceScope?.ServiceProvider ?? throw new InvalidOperationException("ServiceProvider is not initialized.");

    public void Dispose()
    {
        this.Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    protected override void BeginProcessing()
    {
        this.cancellationTokenSource = new CancellationTokenSource();

        base.BeginProcessing();

        var topServiceProvider = this.GetServiceProvider();
        this.serviceScope = topServiceProvider.CreateScope();
        this.Services.GetRequiredService<ICurrentCommandStore>().SetCurrentCommand(this);
        this.ConfigureLogger(this.Services.GetRequiredService<ILoggerFactory>());
    }

    protected virtual void ConfigureLogger(ILoggerFactory loggerFactory) =>
        loggerFactory.AddPowerShell(this.Services);

    protected virtual void Dispose(bool disposing)
    {
        if (!this.disposedValue)
        {
            if (disposing)
            {
                this.cancellationTokenSource.Dispose();
                this.serviceScope?.Dispose();
            }

            this.disposedValue = true;
        }
    }

    protected abstract IServiceProvider GetServiceProvider();

    protected override void ProcessRecord()
#pragma warning disable VSTHRD002 // Avoid problematic synchronous waits
        => this.ProcessRecordAsync(this.cancellationTokenSource.Token).GetAwaiter().GetResult();
#pragma warning restore VSTHRD002 // Avoid problematic synchronous waits

    protected abstract Task ProcessRecordAsync(CancellationToken cancellationToken);

    protected override void StopProcessing()
    {
        this.cancellationTokenSource.Cancel();
        base.StopProcessing();
        this.Dispose();
    }
}
