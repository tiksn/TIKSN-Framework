namespace TIKSN.PowerShell;

public interface ICurrentCommandStore
{
    void SetCurrentCommand(CommandBase command);
}
