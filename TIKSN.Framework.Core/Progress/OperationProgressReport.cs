namespace TIKSN.Progress;

public class OperationProgressReport : ProgressReport
{
    public OperationProgressReport(
        double percentComplete,
        string? currentOperation = null,
        string? statusDescription = null) : base(percentComplete)
    {
        this.CurrentOperation = currentOperation;
        this.StatusDescription = statusDescription;
    }

    public OperationProgressReport(
        int completed,
        int overall,
        string? currentOperation = null,
        string? statusDescription = null) : base(completed, overall)
    {
        this.CurrentOperation = currentOperation;
        this.StatusDescription = statusDescription;
    }

    public string? CurrentOperation { get; set; }

    public string? StatusDescription { get; set; }
}
