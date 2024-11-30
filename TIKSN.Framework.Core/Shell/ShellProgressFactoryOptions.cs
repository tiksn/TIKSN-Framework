using Spectre.Console;

namespace TIKSN.Shell;

public class ShellProgressFactoryOptions
{
    public ShellProgressFactoryOptions(ProgressContext progressContext)
        => this.ProgressContext = progressContext ?? throw new ArgumentNullException(nameof(progressContext));

    public ProgressContext ProgressContext { get; }
}
