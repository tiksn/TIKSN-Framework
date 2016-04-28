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

		public IProgress<ProgressStatus<TStatus>> Progress { get; private set; }

		public TPayload Payload { get; private set; }
	}
}
