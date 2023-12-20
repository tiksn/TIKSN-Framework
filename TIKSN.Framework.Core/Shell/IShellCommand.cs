namespace TIKSN.Shell;

public interface IShellCommand
{
    Task ExecuteAsync(CancellationToken cancellationToken);
}
