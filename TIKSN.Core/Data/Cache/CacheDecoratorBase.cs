using System;

namespace TIKSN.Data.Cache
{
    public abstract class CacheDecoratorBase<T>
    {
        protected static readonly Type entityType = typeof(T);
    }
}
