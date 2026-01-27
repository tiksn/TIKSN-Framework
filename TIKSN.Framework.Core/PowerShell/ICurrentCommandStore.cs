namespace TIKSN.PowerShell;

public interface ICurrentCommandStore
{
    public void SetCurrentCommand(CommandBase command);
}
