namespace TIKSN.PowerShell;

public interface ICurrentCommandProvider
{
    CommandBase GetCurrentCommand();
}
