namespace TIKSN.UI.ViewModels;

public class PromptViewModel
{
    public PromptViewModel(string title, string description, string acceptButtonText, string cancelButtonText,
        string? placeholder = null, string? initialValue = null)
    {
        this.Title = title;
        this.Description = description;
        this.AcceptButtonText = acceptButtonText;
        this.CancelButtonText = cancelButtonText;
        this.Placeholder = placeholder;
        this.InitialValue = initialValue;
    }

    public string AcceptButtonText { get; }
    public string CancelButtonText { get; }
    public string Description { get; }
    public string? InitialValue { get; }
    public string? Placeholder { get; }

    public string Title { get; }
}
