using System.Reactive;
using ReactiveUI;
using TIKSN.Concurrency;
using TIKSN.UI.ViewModels;

namespace TIKSN.UI.Interactions;

public class DialogInteractionService : IDialogInteractionService
{
    public DialogInteractionService(ISequencers sequencers)
    {
        ArgumentNullException.ThrowIfNull(sequencers);

        this.ShowAlert = new Interaction<AlertViewModel, Unit>(sequencers.MainThreadSequencer);
        this.Confirm = new Interaction<ConfirmationViewModel, bool>(sequencers.MainThreadSequencer);
        this.Prompt = new Interaction<PromptViewModel, string?>(sequencers.MainThreadSequencer);
        this.PickFile = new Interaction<FilePickerViewModel, string?>(sequencers.MainThreadSequencer);
        this.PickFiles = new Interaction<FilePickerViewModel, IReadOnlyList<string>>(sequencers.MainThreadSequencer);
        this.PickFolder = new Interaction<FolderPickerViewModel, string?>(sequencers.MainThreadSequencer);
        this.Toast = new Interaction<ToastViewModel, Unit>(sequencers.MainThreadSequencer);
        this.Snackbar = new Interaction<SnackbarViewModel, bool>(sequencers.MainThreadSequencer);
        this.SaveFile = new Interaction<FileSaverViewModel, string?>(sequencers.MainThreadSequencer);
    }

    public Interaction<ConfirmationViewModel, bool> Confirm { get; }

    public Interaction<FilePickerViewModel, string?> PickFile { get; }

    public Interaction<FilePickerViewModel, IReadOnlyList<string>> PickFiles { get; }

    public Interaction<FolderPickerViewModel, string?> PickFolder { get; }

    public Interaction<PromptViewModel, string?> Prompt { get; }

    public Interaction<FileSaverViewModel, string?> SaveFile { get; }

    public Interaction<AlertViewModel, Unit> ShowAlert { get; }

    public Interaction<SnackbarViewModel, bool> Snackbar { get; }

    public Interaction<ToastViewModel, Unit> Toast { get; }
}
