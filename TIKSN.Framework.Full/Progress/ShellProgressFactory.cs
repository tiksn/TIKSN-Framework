using Microsoft.Extensions.Options;
using TIKSN.Shell;

namespace TIKSN.Progress
{
    public class ShellProgressFactory : IOperationProgressFactory
    {
        private readonly IOptions<ShellProgressFactoryOptions> options;

        public ShellProgressFactory(IOptions<ShellProgressFactoryOptions> options)
        {
            this.options = options;
        }

        public DisposableProgress<OperationProgressReport> Create(string activity, string statusDescription)
        {
            return new ShellProgress($"{activity} {statusDescription}", options.Value.Accuracy);
        }
    }
}
