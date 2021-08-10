using System;
using TIKSN.Progress;

namespace TIKSN.PowerShell
{
    public class PowerShellProgressFactory : IOperationProgressFactory
    {
        private readonly ICurrentCommandProvider _currentCommandProvider;

        public PowerShellProgressFactory(ICurrentCommandProvider currentCommandProvider) =>
            this._currentCommandProvider = currentCommandProvider ??
                                           throw new ArgumentNullException(nameof(currentCommandProvider));

        public DisposableProgress<OperationProgressReport> Create(string activity, string statusDescription) =>
            new PowerShellProgress(this._currentCommandProvider, activity, statusDescription);
    }
}
