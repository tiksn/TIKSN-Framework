namespace TIKSN.UI.ViewModels;

public class ConfirmationViewModel
{
    public ConfirmationViewModel(string title, string description, string acceptButtonText, string cancelButtonText)
    {
        this.Title = title;
        this.Description = description;
        this.AcceptButtonText = acceptButtonText;
        this.CancelButtonText = cancelButtonText;
    }

    public string AcceptButtonText { get; }
    public string CancelButtonText { get; }
    public string Description { get; }

    public string Title { get; }
}
