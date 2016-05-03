using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace TIKSN.Progress
{
	public class CompositeWeightedProgress<TStatus> : Progress<ProgressStatus<TStatus>>
	{
		private readonly List<ProgressItem> progresses;
		private readonly List<IProgress<ProgressStatus<TStatus>>> subscribers;

		public CompositeWeightedProgress()
		{
			progresses = new List<ProgressItem>();
			subscribers = new List<IProgress<ProgressStatus<TStatus>>>();
		}

		public CompositeWeightedProgress(Action<ProgressStatus<TStatus>> handler) : base(handler)
		{
			progresses = new List<ProgressItem>();
			subscribers = new List<IProgress<ProgressStatus<TStatus>>>();
		}

		public CompositeWeightedProgress(IProgress<ProgressStatus<TStatus>> subscriber) : this()
		{
			Subscribe(subscriber);
		}

		public IProgress<ProgressStatus<TStatus>> AddProgress(int weight = 1)
		{
			if (weight <= 0)
				throw new ArgumentOutOfRangeException(nameof(weight), "Weight cannot be less than equal to 0");

			var progress = new ProgressItem(weight, SingleProgressItemHandler);

			progresses.Add(progress);

			return progress;
		}

		public void Subscribe(IProgress<ProgressStatus<TStatus>> subscriber)
		{
			subscribers.Add(subscriber);
		}

		protected override void OnReport(ProgressStatus<TStatus> value)
		{
			base.OnReport(value);

			foreach (var subscriber in subscribers)
			{
				subscriber.Report(value);
			}
		}

		private void SingleProgressItemHandler(ProgressStatus<TStatus> status)
		{
			var overallWeight = progresses.Sum(item => item.Weight);
			var currentWeightedProgress = overallWeight == 0 ? 0d : progresses.Sum(item => item.Weight * item.Status.Percentage) / overallWeight;

			Debug.Assert(currentWeightedProgress >= 0d);
			Debug.Assert(currentWeightedProgress <= 100d);

			OnReport(new ProgressStatus<TStatus>(status.Status, currentWeightedProgress));
		}

		private class ProgressItem : Progress<ProgressStatus<TStatus>>
		{
			public ProgressItem(int weight, Action<ProgressStatus<TStatus>> handler) : base(handler)
			{
				Weight = weight;
				Status = new ProgressStatus<TStatus>(default(TStatus), 0d);
			}

			public ProgressStatus<TStatus> Status { get; private set; }

			public int Weight { get; private set; }

			protected override void OnReport(ProgressStatus<TStatus> value)
			{
				Status = value;
				base.OnReport(value);
			}
		}
	}
}