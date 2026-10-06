namespace TIKSN.Shell.AI;

public sealed record ShellCommandSuggestion(
    string CommandName,
    string Reason,
    IReadOnlyDictionary<string, string> Parameters);
