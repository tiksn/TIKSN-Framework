namespace TIKSN.Serialization;

/// <summary>
///     Deserializer interface
/// </summary>
/// <typeparam name="TSerial">Type to deserialize from, usually string or byte array</typeparam>
public interface IDeserializer<in TSerial> where TSerial : class
{
    /// <summary>
    ///     Deserialize from <typeparamref name="TSerial" /> type
    /// </summary>
    /// <typeparam name="T">Type to be deserialized to</typeparam>
    /// <param name="serial">Object to be deserialized</param>
    /// <returns></returns>
    T Deserialize<T>(TSerial serial);
}
