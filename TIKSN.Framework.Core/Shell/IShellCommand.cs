namespace TIKSN.Shell;

public interface IShellCommand
{
    public Task ExecuteAsync(CancellationToken cancellationToken);
}
