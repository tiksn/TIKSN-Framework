using System;

namespace TIKSN.Data
{
    public class Entity<T> : IEntity<T> where T : IEquatable<T>
    {
        public virtual T ID { get; }
    }
}
