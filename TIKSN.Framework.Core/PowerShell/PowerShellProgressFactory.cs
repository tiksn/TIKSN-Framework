using TIKSN.Progress;

namespace TIKSN.PowerShell;

public class PowerShellProgressFactory : IOperationProgressFactory
{
    private readonly ICurrentCommandProvider currentCommandProvider;

    public PowerShellProgressFactory(ICurrentCommandProvider currentCommandProvider) =>
        this.currentCommandProvider = currentCommandProvider ??
                                       throw new ArgumentNullException(nameof(currentCommandProvider));

    public IDisposableProgress<OperationProgressReport> Create(string activity, string statusDescription) =>
        new PowerShellProgress(this.currentCommandProvider, activity, statusDescription);
}
