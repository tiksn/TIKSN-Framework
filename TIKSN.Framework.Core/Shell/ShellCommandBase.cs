namespace TIKSN.Shell;

public abstract class ShellCommandBase : IShellCommand
{
    private readonly IConsoleService consoleService;

    protected ShellCommandBase(IConsoleService consoleService) => this.consoleService = consoleService;

    public abstract Task ExecuteAsync(CancellationToken cancellationToken);

    protected void WriteObject<T>(T tableValue) => this.consoleService.WriteObject(tableValue);

    protected void WriteObjects<T>(IEnumerable<T> tableValues) => this.consoleService.WriteObjects(tableValues);
}
