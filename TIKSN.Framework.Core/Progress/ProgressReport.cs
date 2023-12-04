using System.Diagnostics;

namespace TIKSN.Progress;

public class ProgressReport
{
    public ProgressReport(double percentComplete)
    {
        Debug.Assert(percentComplete >= 0d);
        Debug.Assert(percentComplete <= 100d);

        this.PercentComplete = percentComplete;
    }

    public ProgressReport(int completed, int overall)
    {
        Debug.Assert(completed <= overall);
        Debug.Assert(completed >= 0);

        this.PercentComplete = overall > 0 ? completed * 100d / overall : 0d;
    }

    public double PercentComplete { get; }

    public static ProgressReport CreateProgressReportWithPercentage(double percentComplete) => new(percentComplete);
}

//public class ProgressStatus<T> : ProgressStatus
//{
//	public ProgressStatus(T status, double percentage) : base(percentage)
//	{
//		Status = status;
//	}

// public ProgressStatus(T status, int completed, int overall) : base(completed, overall) {
// Status = status; }

//	public T Status { get; private set; }
//}
