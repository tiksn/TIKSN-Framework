using System;

namespace TIKSN.Progress
{
	public class PayloadProgress<TPayload, TStatus> : Progress<ProgressStatus<TStatus>>
	{
		public PayloadProgress(TPayload payload)
		{
			Payload = payload;
			Progress = new ProgressStatus<TStatus>(default(TStatus), 0d);
		}

		public TPayload Payload { get; private set; }

		public ProgressStatus<TStatus> Progress { get; private set; }

		protected override void OnReport(ProgressStatus<TStatus> value)
		{
			if (value == null)
				throw new ArgumentNullException(nameof(value));

			this.Progress = value;

			base.OnReport(value);
		}
	}
}