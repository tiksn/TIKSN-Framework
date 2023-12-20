namespace TIKSN.Serialization;

/// <summary>
///     Custom (specialized or typed) deserializer interface
/// </summary>
/// <typeparam name="TSerial">Type to deserialize from, usually string or byte array</typeparam>
/// <typeparam name="TModel">Type to be deserialized</typeparam>
public interface ICustomDeserializer<in TSerial, out TModel>
{
    /// <summary>
    ///     Deserialize from <typeparamref name="TSerial" /> type to <typeparamref name="TModel" />
    /// </summary>
    /// <param name="serial"></param>
    /// <returns></returns>
    TModel Deserialize(TSerial serial);
}
