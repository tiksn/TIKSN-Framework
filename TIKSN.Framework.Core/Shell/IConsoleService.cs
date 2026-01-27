using System.Security;

namespace TIKSN.Shell;

public interface IConsoleService
{
    public string ReadLine(string promptMessage, ConsoleColor promptForegroundColor);

    public SecureString ReadPasswordLine(string promptMessage, ConsoleColor promptForegroundColor);

    public IDisposable RegisterCancellation(CancellationTokenSource cancellationTokenSource);

    public int UserPrompt(string message, params string[] options);

    public void WriteError(string errorMessage);

    public void WriteObject<T>(T value);

    public void WriteObjects<T>(IEnumerable<T> values);
}
