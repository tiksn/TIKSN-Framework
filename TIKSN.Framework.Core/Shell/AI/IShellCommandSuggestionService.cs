namespace TIKSN.Shell.AI;

public interface IShellCommandSuggestionService
{
    Task<IReadOnlyList<ShellCommandSuggestion>> SuggestAsync(
        string request,
        CancellationToken cancellationToken);
}
