using System.Diagnostics;
using System.Management.Automation;
using TIKSN.Progress;

namespace TIKSN.PowerShell;

public class PowerShellProgress : Progress<OperationProgressReport>, IDisposableProgress<OperationProgressReport>
{
    private static readonly Lock ActivityIdLocker = new();
    private static int nextActivityId;
    private readonly ICurrentCommandProvider currentCommandProvider;
    private readonly ProgressRecord progressRecord;
    private readonly Stopwatch stopwatch;
    private bool disposedValue;

    public PowerShellProgress(ICurrentCommandProvider currentCommandProvider, string activity,
        string statusDescription)
    {
        this.progressRecord = new ProgressRecord(GenerateNextActivityId(), activity, statusDescription);

        this.stopwatch = Stopwatch.StartNew();
        this.progressRecord.RecordType = ProgressRecordType.Processing;
        this.currentCommandProvider = currentCommandProvider
            ?? throw new ArgumentNullException(nameof(currentCommandProvider));
        this.currentCommandProvider.GetCurrentCommand().WriteProgress(this.progressRecord);
    }

    public PowerShellProgress CreateChildProgress(string activity, string statusDescription)
    {
        var childProgress = new PowerShellProgress(this.currentCommandProvider, activity, statusDescription);

        childProgress.progressRecord.ParentActivityId = this.progressRecord.ActivityId;

        return childProgress;
    }

    public void Dispose()
    {
        this.Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!this.disposedValue)
        {
            if (disposing)
            {
                this.stopwatch.Stop();
                this.progressRecord.RecordType = ProgressRecordType.Completed;
                this.currentCommandProvider.GetCurrentCommand().WriteProgress(this.progressRecord);
            }

            this.disposedValue = true;
        }
    }

    protected override void OnReport(OperationProgressReport value)
    {
        ArgumentNullException.ThrowIfNull(value);

        this.progressRecord.RecordType = ProgressRecordType.Processing;
        this.progressRecord.PercentComplete = (int)value.PercentComplete;

        this.progressRecord.SecondsRemaining = (int)(this.stopwatch.Elapsed.TotalSeconds *
            (100d - value.PercentComplete) / value.PercentComplete);

        if (value.CurrentOperation != null)
        {
            this.progressRecord.CurrentOperation = value.CurrentOperation;
        }

        if (value.StatusDescription != null)
        {
            this.progressRecord.StatusDescription = value.StatusDescription;
        }

        this.currentCommandProvider.GetCurrentCommand().WriteProgress(this.progressRecord);
        base.OnReport(value);
    }

    private static int GenerateNextActivityId()
    {
        lock (ActivityIdLocker)
        {
            nextActivityId++;
        }

        return nextActivityId;
    }
}
