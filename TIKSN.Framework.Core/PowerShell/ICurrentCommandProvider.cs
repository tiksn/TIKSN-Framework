namespace TIKSN.PowerShell;

public interface ICurrentCommandProvider
{
    public CommandBase GetCurrentCommand();
}
