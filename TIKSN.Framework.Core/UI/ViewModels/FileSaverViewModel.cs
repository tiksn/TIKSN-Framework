namespace TIKSN.UI.ViewModels;

public class FileSaverViewModel
{
    public required string FileName { get; init; }
    public required Stream Stream { get; init; }
}
