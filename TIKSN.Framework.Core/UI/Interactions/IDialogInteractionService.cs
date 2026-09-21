using System.Reactive;
using ReactiveUI;
using TIKSN.UI.ViewModels;

namespace TIKSN.UI.Interactions;

public interface IDialogInteractionService
{
    public Interaction<ConfirmationViewModel, bool> Confirm { get; }

    public Interaction<FilePickerViewModel, string?> PickFile { get; }

    public Interaction<FilePickerViewModel, IReadOnlyList<string>> PickFiles { get; }

    public Interaction<FolderPickerViewModel, string?> PickFolder { get; }

    public Interaction<PromptViewModel, string?> Prompt { get; }

    public Interaction<AlertViewModel, Unit> ShowAlert { get; }
}
