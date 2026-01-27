namespace TIKSN.Data;

public interface IEntity<T> where T : IEquatable<T>
{
    public T ID { get; }
}
