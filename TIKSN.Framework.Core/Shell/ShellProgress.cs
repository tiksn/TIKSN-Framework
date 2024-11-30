using Spectre.Console;
using TIKSN.Progress;

namespace TIKSN.Shell;

public class ShellProgress : Progress<OperationProgressReport>, IDisposableProgress<OperationProgressReport>
{
    private readonly ProgressTask progressTask;
    private bool disposedValue;

    public ShellProgress(ProgressContext progressContext, string message)
    {
        ArgumentNullException.ThrowIfNull(progressContext);

        this.progressTask = progressContext.AddTask(Markup.Escape(message));
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
                this.progressTask.StopTask();
            }

            this.disposedValue = true;
        }
    }

    protected override void OnReport(OperationProgressReport value)
    {
        ArgumentNullException.ThrowIfNull(value);

        base.OnReport(value);

        this.progressTask.Description = $"{value.StatusDescription} {value.CurrentOperation}";

        this.progressTask.Value = value.PercentComplete;
    }
}
