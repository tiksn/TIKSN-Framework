using ShellProgressBar;
using TIKSN.Progress;

namespace TIKSN.Shell
{
    public class ShellProgress : DisposableProgress<OperationProgressReport>
    {
        private readonly ProgressBar _progressBar;
        private readonly int _accuracy;
        public ShellProgress(string message, int accuracy)
        {
            _accuracy = accuracy;
            var options = new ProgressBarOptions();
            _progressBar = new ProgressBar(EstimateTicks(100d), message, options);
        }

        private int EstimateTicks(double percentage)
        {
            return (int)(percentage * _accuracy);
        }

        protected override void OnReport(OperationProgressReport value)
        {
            base.OnReport(value);

            _progressBar.UpdateMessage($"{value.StatusDescription} {value.CurrentOperation}");

            var updatedTicks = EstimateTicks(value.PercentComplete);

            while (_progressBar.CurrentTick < updatedTicks)
            {
                _progressBar.Tick();
            }
        }

        public override void Dispose()
        {
            _progressBar.Dispose();
        }
    }
}
