using System.Reactive;
using TIKSN.UI.Interactions;

namespace TIKSN.UI;

public static class DialogInteractionHandlers
{
    public static void RegisterDialogHandlers(this IDialogInteractionService service, Page mainPage)
    {
        ArgumentNullException.ThrowIfNull(service);
        ArgumentNullException.ThrowIfNull(mainPage);

        _ = service.ShowAlert.RegisterHandler(async interaction =>
        {
            await mainPage.DisplayAlertAsync(interaction.Input.Title, interaction.Input.Description,
                interaction.Input.ButtonText).ConfigureAwait(false);
            interaction.SetOutput(Unit.Default);
        });

        _ = service.Confirm.RegisterHandler(async interaction =>
        {
            var result = await mainPage.DisplayAlertAsync(interaction.Input.Title, interaction.Input.Description,
                interaction.Input.AcceptButtonText, interaction.Input.CancelButtonText).ConfigureAwait(false);
            interaction.SetOutput(result);
        });

        _ = service.Prompt.RegisterHandler(async interaction =>
        {
            var result = await mainPage.DisplayPromptAsync(interaction.Input.Title, interaction.Input.Description,
                interaction.Input.AcceptButtonText, interaction.Input.CancelButtonText, interaction.Input.Placeholder,
                -1, Keyboard.Default, interaction.Input.InitialValue ?? string.Empty).ConfigureAwait(false);
            interaction.SetOutput(result);
        });

        _ = service.PickFile.RegisterHandler(async interaction =>
        {
            var pickOptions = new PickOptions { PickerTitle = interaction.Input.Title };
            var result = await FilePicker.Default.PickAsync(pickOptions).ConfigureAwait(false);
            interaction.SetOutput(result?.FullPath);
        });

        _ = service.PickFiles.RegisterHandler(async interaction =>
        {
            var pickOptions = new PickOptions { PickerTitle = interaction.Input.Title };
            var results = await FilePicker.Default.PickMultipleAsync(pickOptions).ConfigureAwait(false);
            var paths = results != null
                ? Enumerable.ToArray(Enumerable.Select(results, r => r.FullPath))
                : [];
            interaction.SetOutput(paths);
        });

        _ = service.PickFolder.RegisterHandler(async interaction =>
        {
            var result = await CommunityToolkit.Maui.Storage.FolderPicker.Default
                .PickAsync(interaction.Input.Title, default).ConfigureAwait(false);
            interaction.SetOutput(result.IsSuccessful ? result.Folder.Path : null);
        });
    }
}
