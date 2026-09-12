namespace TIKSN.UI.ViewModels;

public class AlertViewModel
{
    public AlertViewModel(string title, string description, string buttonText)
    {
        this.Title = title;
        this.Description = description;
        this.ButtonText = buttonText;
    }

    public string ButtonText { get; }

    public string Description { get; }

    public string Title { get; }
}
