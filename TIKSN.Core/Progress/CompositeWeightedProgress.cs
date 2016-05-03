using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace TIKSN.Progress
{
	public class CompositeWeightedProgress<TStatus> : Progress<ProgressStatus<TStatus>>
	{
		private readonly List<ProgressItem> progresses;

		public CompositeWeightedProgress()
		{
			progresses = new List<ProgressItem>();
		}

		public IProgress<ProgressStatus<TStatus>> AddProgress(int weight = 1)
		{
			if (weight <= 0)
				throw new ArgumentOutOfRangeException(nameof(weight), "Weight cannot be less than equal to 0");

			var progress = new Progress<ProgressStatus<TStatus>>();
			progress.ProgressChanged += ProgressChanged;

			progresses.Add(new ProgressItem(progress, weight));
			return progress;
		}

		private void ProgressChanged(object sender, ProgressStatus<TStatus> e)
		{
			var currentProgress = (Progress<ProgressStatus<TStatus>>)sender;

			var overallWeight = progresses.Sum(item => item.Weight);

			var currentWeightedProgress = progresses.Sum(item => item.Weight * item.Status.Percentage);

			currentWeightedProgress = overallWeight == 0 ? 0d : currentWeightedProgress / overallWeight;

			Debug.Assert(currentWeightedProgress >= 0d);
			Debug.Assert(currentWeightedProgress <= 100d);

			OnReport(new ProgressStatus<TStatus>(e.Status, currentWeightedProgress));
		}

		private class ProgressItem
		{
			public ProgressItem(Progress<ProgressStatus<TStatus>> progress, int weight)
			{
				Weight = weight;
				Progress = progress;
				Status = new ProgressStatus<TStatus>(default(TStatus), 0d);
				progress.ProgressChanged += ProgressChanged;
			}

			public IProgress<ProgressStatus<TStatus>> Progress { get; private set; }

			public ProgressStatus<TStatus> Status { get; private set; }

			public int Weight { get; private set; }

			private void ProgressChanged(object sender, ProgressStatus<TStatus> e)
			{
				Status = e;
			}
		}
	}
}