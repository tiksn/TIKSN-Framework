using System.Security;

namespace TIKSN.Shell;

public interface IConsoleService
{
    string ReadLine(string promptMessage, ConsoleColor promptForegroundColor);

    SecureString ReadPasswordLine(string promptMessage, ConsoleColor promptForegroundColor);

    IDisposable RegisterCancellation(CancellationTokenSource cancellationTokenSource);

    int UserPrompt(string message, params string[] options);

    void WriteError(string errorMessage);

    void WriteObject<T>(T value);

    void WriteObjects<T>(IEnumerable<T> values);
}
