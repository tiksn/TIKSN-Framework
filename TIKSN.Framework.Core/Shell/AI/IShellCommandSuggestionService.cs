namespace TIKSN.Shell.AI;

public interface IShellCommandSuggestionService
{
    public Task<IReadOnlyList<ShellCommandSuggestion>> SuggestAsync(
        string request,
        CancellationToken cancellationToken);
}
