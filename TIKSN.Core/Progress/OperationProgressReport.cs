﻿namespace TIKSN.Progress
{
	public class OperationProgressReport : ProgressReport
	{
		public OperationProgressReport(double percentComplete, string currentOperation = null, string statusDescription = null) : base(percentComplete)
		{
			CurrentOperation = currentOperation;
			StatusDescription = statusDescription;
		}

		public OperationProgressReport(int completed, int overall, string currentOperation = null, string statusDescription = null) : base(completed, overall)
		{
			CurrentOperation = currentOperation;
			StatusDescription = statusDescription;
		}

		public string CurrentOperation { get; set; }

		//public string Activity { get; set; }
		//public int ActivityId { get; }
		//public int ParentActivityId { get; set; }
		//public ProgressRecordType RecordType { get; set; }
		public string StatusDescription { get; set; }
	}
}