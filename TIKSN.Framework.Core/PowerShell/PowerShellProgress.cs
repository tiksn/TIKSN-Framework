using System.Diagnostics;
using System.Management.Automation;
using TIKSN.Progress;

namespace TIKSN.PowerShell;

public class PowerShellProgress : DisposableProgress<OperationProgressReport>
{
    private static readonly object activityIdLocker = new();
    private static int nextActivityId;

    private readonly ICurrentCommandProvider _currentCommandProvider;
    private readonly ProgressRecord progressRecord;
    private readonly Stopwatch stopwatch;

    public PowerShellProgress(ICurrentCommandProvider currentCommandProvider, string activity,
        string statusDescription)
    {
        this.progressRecord = new ProgressRecord(GenerateNextActivityId(), activity, statusDescription);

        this.stopwatch = Stopwatch.StartNew();
        this.progressRecord.RecordType = ProgressRecordType.Processing;
        this._currentCommandProvider = currentCommandProvider ??
                                       throw new ArgumentNullException(nameof(currentCommandProvider));
        this._currentCommandProvider.GetCurrentCommand().WriteProgress(this.progressRecord);
    }

    public PowerShellProgress CreateChildProgress(string activity, string statusDescription)
    {
        var childProgress = new PowerShellProgress(this._currentCommandProvider, activity, statusDescription);

        childProgress.progressRecord.ParentActivityId = this.progressRecord.ActivityId;

        return childProgress;
    }

    public override void Dispose()
    {
        this.stopwatch.Stop();
        this.progressRecord.RecordType = ProgressRecordType.Completed;
        this._currentCommandProvider.GetCurrentCommand().WriteProgress(this.progressRecord);
    }

    protected override void OnReport(OperationProgressReport value)
    {
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

        this._currentCommandProvider.GetCurrentCommand().WriteProgress(this.progressRecord);
        base.OnReport(value);
    }

    private static int GenerateNextActivityId()
    {
        lock (activityIdLocker)
        {
            nextActivityId++;
        }

        return nextActivityId;
    }
}
