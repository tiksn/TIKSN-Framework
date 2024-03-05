using System.Collections.Concurrent;

namespace TIKSN.PowerShell;

public class PowerShellLoggerScopeDisposable : IDisposable
{
    private readonly ConcurrentStack<object> scopes;
    private bool disposedValue;

    public PowerShellLoggerScopeDisposable(ConcurrentStack<object> scopes) => this.scopes = scopes;

    public void Dispose()
    {
        this.Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!this.disposedValue)
        {
            if (disposing)
            {
                _ = this.scopes.TryPop(out _);
            }

            this.disposedValue = true;
        }
    }
}
