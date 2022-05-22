using System;
using System.Collections.Concurrent;

namespace TIKSN.PowerShell
{
    public class PowerShellLoggerScopeDisposable : IDisposable
    {
        private readonly ConcurrentStack<object> scopes;

        public PowerShellLoggerScopeDisposable(ConcurrentStack<object> scopes) => this.scopes = scopes;

        public void Dispose() => _ = this.scopes.TryPop(out _);
    }
}
