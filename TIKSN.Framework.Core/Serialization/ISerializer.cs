namespace TIKSN.Serialization;

/// <summary>
///     Serializer interface
/// </summary>
/// <typeparam name="TSerial">Type to serialize to, usually string or byte array</typeparam>
public interface ISerializer<out TSerial> where TSerial : class
{
    /// <summary>
    ///     Serialize to <typeparamref name="TSerial" /> type
    /// </summary>
    /// <typeparam name="T">Type to be serialized</typeparam>
    /// <param name="obj">Object to be serialized</param>
    /// <returns>Serialized result</returns>
    public TSerial Serialize<T>(T obj);
}
