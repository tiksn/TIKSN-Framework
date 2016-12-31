using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace TIKSN.Progress
{
	public class CompositeWeightedProgress<T> : Progress<T> where T : ProgressReport
	{
		private readonly Func<T, double, T> createProgressReportWithPercentage;
		private readonly List<ProgressItem> progresses;
		private readonly List<IProgress<T>> subscribers;

		public CompositeWeightedProgress(Func<T, double, T> createProgressReportWithPercentage)
		{
			progresses = new List<ProgressItem>();
			subscribers = new List<IProgress<T>>();
			this.createProgressReportWithPercentage = createProgressReportWithPercentage;
		}

		public CompositeWeightedProgress(Action<T> handler, Func<T, double, T> createProgressReportWithPercentage) : base(handler)
		{
			progresses = new List<ProgressItem>();
			subscribers = new List<IProgress<T>>();
			this.createProgressReportWithPercentage = createProgressReportWithPercentage;
		}

		public CompositeWeightedProgress(IProgress<T> subscriber, Func<T, double, T> createProgressReportWithPercentage) : this(createProgressReportWithPercentage)
		{
			Subscribe(subscriber);
		}

		public IProgress<T> AddProgress(int weight = 1)
		{
			if (weight <= 0)
				throw new ArgumentOutOfRangeException(nameof(weight), "Weight cannot be less than equal to 0");

			var progress = new ProgressItem(weight, SingleProgressItemHandler);

			progresses.Add(progress);

			return progress;
		}

		public void Subscribe(IProgress<T> subscriber)
		{
			subscribers.Add(subscriber);
		}

		protected override void OnReport(T value)
		{
			base.OnReport(value);

			foreach (var subscriber in subscribers)
			{
				subscriber.Report(value);
			}
		}

		private void SingleProgressItemHandler(T status)
		{
			if (status == null)
				throw new ArgumentNullException(nameof(status));

			var overallWeight = progresses.Sum(item => item.Weight);
			var currentWeightedProgress = overallWeight == 0 ? 0d : progresses.Sum(item => item.Weight * (item.Status == null ? 0d : item.Status.PercentComplete)) / overallWeight;

			Debug.Assert(currentWeightedProgress >= 0d);
			Debug.Assert(currentWeightedProgress <= 100d);

			OnReport(createProgressReportWithPercentage(status, currentWeightedProgress));
		}

		private class ProgressItem : Progress<T>
		{
			public ProgressItem(int weight, Action<T> handler) : base(handler)
			{
				Weight = weight;
				Status = default(T);
			}

			public T Status { get; private set; }

			public int Weight { get; private set; }

			protected override void OnReport(T value)
			{
				Status = value;
				base.OnReport(value);
			}
		}
	}
}