namespace TIKSN.PowerShell;

public class CurrentCommandContext : ICurrentCommandStore, ICurrentCommandProvider
{
    private CommandBase _command;

    public CommandBase GetCurrentCommand()
    {
        if (this._command == null)
        {
            throw new NullReferenceException("Command is not set yet.");
        }

        return this._command;
    }

    public void SetCurrentCommand(CommandBase command) => this._command = command ?? throw new ArgumentNullException(nameof(command));
}
