using System;

namespace TIKSN.Progress
{
    public abstract class DisposableProgress<T> : Progress<T>, IDisposable
    {
        public abstract void Dispose();
    }
}
