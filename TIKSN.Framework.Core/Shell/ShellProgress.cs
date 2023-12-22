using ShellProgressBar;
using TIKSN.Progress;

namespace TIKSN.Shell;

public class ShellProgress : Progress<OperationProgressReport>, IDisposableProgress<OperationProgressReport>
{
    private readonly int accuracy;
    private readonly ProgressBar progressBar;
    private bool disposedValue;

    public ShellProgress(string message, int accuracy)
    {
        this.accuracy = accuracy;
        var options = new ProgressBarOptions();
        this.progressBar = new ProgressBar(this.EstimateTicks(100d), message, options);
    }

    public void Dispose()
    {
        this.Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!this.disposedValue)
        {
            if (disposing)
            {
                this.progressBar.Dispose();
            }

            this.disposedValue = true;
        }
    }

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
