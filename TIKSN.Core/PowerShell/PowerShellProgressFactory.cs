using System;
using TIKSN.Progress;

namespace TIKSN.PowerShell
{
    public class PowerShellProgressFactory : IOperationProgressFactory
    {
        private readonly ICurrentCommandProvider _currentCommandProvider;

        public PowerShellProgressFactory(ICurrentCommandProvider currentCommandProvider)
        {
            _currentCommandProvider = currentCommandProvider ?? throw new ArgumentNullException(nameof(currentCommandProvider));
        }

        public DisposableProgress<OperationProgressReport> Create(string activity, string statusDescription)
        {
            return new PowerShellProgress(_currentCommandProvider, activity, statusDescription);
        }
    }
}