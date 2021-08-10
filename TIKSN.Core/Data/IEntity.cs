using System;

namespace TIKSN.Data
{
    public interface IEntity<T> where T : IEquatable<T>
    {
        T ID { get; }
    }
}
