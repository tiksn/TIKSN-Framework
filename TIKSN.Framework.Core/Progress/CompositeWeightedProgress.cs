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
            this.progresses = new List<ProgressItem>();
            this.subscribers = new List<IProgress<T>>();
            this.createProgressReportWithPercentage = createProgressReportWithPercentage;
        }

        public CompositeWeightedProgress(Action<T> handler, Func<T, double, T> createProgressReportWithPercentage) :
            base(handler)
        {
            this.progresses = new List<ProgressItem>();
            this.subscribers = new List<IProgress<T>>();
            this.createProgressReportWithPercentage = createProgressReportWithPercentage;
        }

        public CompositeWeightedProgress(IProgress<T> subscriber, Func<T, double, T> createProgressReportWithPercentage)
            : this(createProgressReportWithPercentage) => this.Subscribe(subscriber);

        public IProgress<T> AddProgress(int weight = 1)
        {
            if (weight <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(weight), "Weight cannot be less than equal to 0");
            }

            var progress = new ProgressItem(weight, this.SingleProgressItemHandler);

            this.progresses.Add(progress);

            return progress;
        }

        public void Subscribe(IProgress<T> subscriber) => this.subscribers.Add(subscriber);

        protected override void OnReport(T value)
        {
            base.OnReport(value);

            foreach (var subscriber in this.subscribers)
            {
                subscriber.Report(value);
            }
        }

        private void SingleProgressItemHandler(T status)
        {
            if (status == null)
            {
                throw new ArgumentNullException(nameof(status));
            }

            var overallWeight = this.progresses.Sum(item => item.Weight);
            var currentWeightedProgress = overallWeight == 0
                ? 0d
                : this.progresses.Sum(item => item.Weight * ((item.Status?.PercentComplete) ?? 0d)) /
                  overallWeight;

            Debug.Assert(currentWeightedProgress >= 0d);
            Debug.Assert(currentWeightedProgress <= 100d);

            this.OnReport(this.createProgressReportWithPercentage(status, currentWeightedProgress));
        }

        private sealed class ProgressItem : Progress<T>
        {
            public ProgressItem(int weight, Action<T> handler) : base(handler)
            {
                this.Weight = weight;
                this.Status = default;
            }

            public T Status { get; private set; }

            public int Weight { get; }

            protected override void OnReport(T value)
            {
                this.Status = value;
                base.OnReport(value);
            }
        }
    }
}
