using System;
using System.Diagnostics;
using System.Management.Automation;

namespace TIKSN.Progress
{
	public class PowerShellProgress : Progress<OperationProgressReport>
	{
		private readonly Cmdlet cmdlet;
		private readonly ProgressRecord progressRecord;
		private Stopwatch stopwatch;

		public PowerShellProgress(Cmdlet cmdlet, int activityId, string activity, string statusDescription)
		{
			this.cmdlet = cmdlet;
			this.progressRecord = new ProgressRecord(activityId, activity, statusDescription);
		}

		protected override void OnReport(OperationProgressReport value)
		{
			Start();

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

		private void Start()
		{
			if (stopwatch == null)
			{
				stopwatch = Stopwatch.StartNew();
			}
		}
	}
}