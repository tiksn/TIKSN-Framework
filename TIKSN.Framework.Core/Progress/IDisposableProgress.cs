namespace TIKSN.Progress;

public interface IDisposableProgress<in T> : IProgress<T>, IDisposable
{
}
