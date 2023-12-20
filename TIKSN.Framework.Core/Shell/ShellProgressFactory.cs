using Microsoft.Extensions.Options;
using TIKSN.Progress;

namespace TIKSN.Shell;

public class ShellProgressFactory : IOperationProgressFactory
{
    private readonly IOptions<ShellProgressFactoryOptions> options;

    public ShellProgressFactory(IOptions<ShellProgressFactoryOptions> options) => this.options = options;

    public DisposableProgress<OperationProgressReport> Create(string activity, string statusDescription) =>
        new ShellProgress($"{activity} {statusDescription}", this.options.Value.Accuracy);
}
