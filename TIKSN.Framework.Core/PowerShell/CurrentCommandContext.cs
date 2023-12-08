namespace TIKSN.PowerShell;

public class CurrentCommandContext : ICurrentCommandStore, ICurrentCommandProvider
{
    private CommandBase command;

    public CommandBase GetCurrentCommand()
    {
        if (this.command == null)
        {
            throw new InvalidOperationException("Command is not set yet.");
        }

        return this.command;
    }

    public void SetCurrentCommand(CommandBase command) => this.command = command ?? throw new ArgumentNullException(nameof(command));
}
