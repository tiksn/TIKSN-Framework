using ShellProgressBar;
using TIKSN.Progress;

namespace TIKSN.Shell;

public class ShellProgress : DisposableProgress<OperationProgressReport>
{
    private readonly int _accuracy;
    private readonly ProgressBar _progressBar;

    public ShellProgress(string message, int accuracy)
    {
        this._accuracy = accuracy;
        var options = new ProgressBarOptions();
        this._progressBar = new ProgressBar(this.EstimateTicks(100d), message, options);
    }

    private int EstimateTicks(double percentage) => (int)(percentage * this._accuracy);

    protected override void OnReport(OperationProgressReport value)
    {
        base.OnReport(value);

        this._progressBar.Message = $"{value.StatusDescription} {value.CurrentOperation}";

        var updatedTicks = this.EstimateTicks(value.PercentComplete);

        while (this._progressBar.CurrentTick < updatedTicks)
        {
            this._progressBar.Tick();
        }
    }

    public override void Dispose() => this._progressBar.Dispose();
}
