using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace TIKSN.Progress
{
	public class CompositeWeightedProgress<TPayload, TStatus> : Progress<ProgressStatus<TStatus>>
	{
		private List<Tuple<PayloadProgress<TPayload, TStatus>, int>> progresses;

		public CompositeWeightedProgress()
		{
			progresses = new List<Tuple<PayloadProgress<TPayload, TStatus>, int>>();
		}

		public IEnumerable<ProgressData<TPayload, TStatus>> Progresses
		{
			get
			{
				return progresses.Select(item => new ProgressData<TPayload, TStatus>(item.Item1, item.Item1.Payload));
			}
		}

		public void AddProgress(TPayload payload, int weight = 1)
		{
			if (weight <= 0)
				throw new ArgumentOutOfRangeException(nameof(weight), "Weight cannot be less than equal to 0");

			var progress = new PayloadProgress<TPayload, TStatus>(payload);
			progress.ProgressChanged += progress_ProgressChanged;

			progresses.Add(new Tuple<PayloadProgress<TPayload, TStatus>, int>(progress, weight));
		}

		private void progress_ProgressChanged(object sender, ProgressStatus<TStatus> e)
		{
			var currentProgress = (PayloadProgress<TPayload, TStatus>)sender;

			var overallWeight = progresses.Sum(item => item.Item2);

			var currentWeightedProgress = progresses.Sum(item => item.Item2 * item.Item1.Progress.Percentage);

			currentWeightedProgress = overallWeight == 0 ? 0d : currentWeightedProgress / overallWeight;

			Debug.Assert(currentWeightedProgress >= 0d);
			Debug.Assert(currentWeightedProgress <= 100d);

			OnReport(new ProgressStatus<TStatus>(currentProgress.Progress.Status, currentWeightedProgress));
		}
	}
}