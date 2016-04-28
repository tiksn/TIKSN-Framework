using System;

namespace TIKSN.Progress
{
	public class ProgressData<TPayload, TStatus>
	{
		public ProgressData(IProgress<ProgressStatus<TStatus>> progress, TPayload payload)
		{
			Progress = progress;
			Payload = payload;
		}

		public TPayload Payload { get; private set; }
		public IProgress<ProgressStatus<TStatus>> Progress { get; private set; }
	}
}