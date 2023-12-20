using ShellProgressBar;
using TIKSN.Progress;

namespace TIKSN.Shell;

public class ShellProgress : DisposableProgress<OperationProgressReport>
{
    private readonly int accuracy;
    private readonly ProgressBar progressBar;

    public ShellProgress(string message, int accuracy)
    {
        this.accuracy = accuracy;
        var options = new ProgressBarOptions();
        this.progressBar = new ProgressBar(this.EstimateTicks(100d), message, options);
    }

    public override void Dispose() => this.progressBar.Dispose();

    protected override void OnReport(OperationProgressReport value)
    {
        ArgumentNullException.ThrowIfNull(value);

        base.OnReport(value);

        this.progressBar.Message = $"{value.StatusDescription} {value.CurrentOperation}";

        var updatedTicks = this.EstimateTicks(value.PercentComplete);

        while (this.progressBar.CurrentTick < updatedTicks)
        {
            this.progressBar.Tick();
        }
    }

    private int EstimateTicks(double percentage) => (int)(percentage * this.accuracy);
}
