using System;
using System.Diagnostics;
using System.Management.Automation;

namespace TIKSN.Progress
{
    public class PowerShellProgress : Progress<OperationProgressReport>, IDisposable
    {
        private static readonly object activityIdLocker = new object();
        private static int nextActivityId = 0;

        private readonly Cmdlet cmdlet;
        private readonly ProgressRecord progressRecord;
        private Stopwatch stopwatch;

        public PowerShellProgress(Cmdlet cmdlet, string activity, string statusDescription)
        {
            this.cmdlet = cmdlet;
            this.progressRecord = new ProgressRecord(GenerateNextActivityId(), activity, statusDescription);

            stopwatch = Stopwatch.StartNew();
            progressRecord.RecordType = ProgressRecordType.Processing;
            cmdlet.WriteProgress(progressRecord);
        }

        public PowerShellProgress CreateChildProgress(string activity, string statusDescription)
        {
            var childProgress = new PowerShellProgress(cmdlet, activity, statusDescription);

            childProgress.progressRecord.ParentActivityId = progressRecord.ActivityId;

            return childProgress;
        }

        public void Dispose()
        {
            stopwatch.Stop();
            progressRecord.RecordType = ProgressRecordType.Completed;
            cmdlet.WriteProgress(progressRecord);
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

            cmdlet.WriteProgress(progressRecord);
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