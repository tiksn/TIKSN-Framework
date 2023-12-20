namespace TIKSN.Shell;

public class ShellCommandSuspendedException : Exception
{
    public ShellCommandSuspendedException()
    {
    }

    public ShellCommandSuspendedException(string message) : base(message)
    {
    }

    public ShellCommandSuspendedException(string message, Exception inner) : base(message, inner)
    {
    }
}
