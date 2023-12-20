namespace TIKSN.Serialization;

public abstract class DeserializerBase<TSerial> : IDeserializer<TSerial> where TSerial : class
{
    public T Deserialize<T>(TSerial serial)
    {
        try
        {
            return this.DeserializeInternal<T>(serial);
        }
        catch (Exception ex)
        {
            throw new DeserializerException("Deserialization failed.", ex);
        }
    }

    protected abstract T DeserializeInternal<T>(TSerial serial);
}
