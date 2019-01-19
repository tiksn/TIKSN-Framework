using System;
using System.Diagnostics;
using System.Management.Automation;
using TIKSN.Progress;

namespace TIKSN.PowerShell
{
    public class PowerShellProgress : DisposableProgress<OperationProgressReport>
    {
        private static readonly object activityIdLocker = new object();
        private static int nextActivityId = 0;

        private readonly ICurrentCommandProvider _currentCommandProvider;
        private readonly ProgressRecord progressRecord;
        private Stopwatch stopwatch;

        public PowerShellProgress(ICurrentCommandProvider currentCommandProvider, string activity, string statusDescription)
        {
            this.progressRecord = new ProgressRecord(GenerateNextActivityId(), activity, statusDescription);

            stopwatch = Stopwatch.StartNew();
            progressRecord.RecordType = ProgressRecordType.Processing;
            _currentCommandProvider = currentCommandProvider ?? throw new ArgumentNullException(nameof(currentCommandProvider));
            _currentCommandProvider.GetCurrentCommand().WriteProgress(progressRecord);
        }

        public PowerShellProgress CreateChildProgress(string activity, string statusDescription)
        {
            var childProgress = new PowerShellProgress(_currentCommandProvider, activity, statusDescription);

            childProgress.progressRecord.ParentActivityId = progressRecord.ActivityId;

            return childProgress;
        }

        public override void Dispose()
        {
            stopwatch.Stop();
            progressRecord.RecordType = ProgressRecordType.Completed;
            _currentCommandProvider.GetCurrentCommand().WriteProgress(progressRecord);
        }

        protected override void OnReport(OperationProgressReport value)
        {
            progressRecord.RecordType = ProgressRecordType.Processing;
            progressRecord.PercentComplete = (int)value.PercentComplete;

            progressRecord.SecondsRemaining = (int)(stopwatch.Elapsed.TotalSeconds * (100d - value.PercentComplete) / value.PercentComplete);

            if (value.CurrentOperation != null)
            {
                progressRecord.CurrentOperation = value.CurrentOperation;
            }

            if (value.StatusDescription != null)
            {
                progressRecord.StatusDescription = value.StatusDescription;
            }

            _currentCommandProvider.GetCurrentCommand().WriteProgress(progressRecord);
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
}