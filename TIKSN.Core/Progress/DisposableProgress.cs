using System;

namespace TIKSN.Progress
{
    public class DisposableProgress<T> : Progress<T>, IDisposable
    {
        public virtual void Dispose()
        {
        }
    }
}
