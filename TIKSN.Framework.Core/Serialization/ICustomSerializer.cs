namespace TIKSN.Serialization;

/// <summary>
///     Custom (specialized or typed) serializer interface
/// </summary>
/// <typeparam name="TSerial">Type to serialize to, usually string or byte array</typeparam>
/// <typeparam name="TModel">Type to be serialized</typeparam>
public interface ICustomSerializer<out TSerial, in TModel>
{
    /// <summary>
    ///     Serialize <typeparamref name="TModel" /> to <typeparamref name="TSerial" /> type
    /// </summary>
    /// <param name="obj">Model to serialized</param>
    /// <returns>Serialized result</returns>
    TSerial Serialize(TModel obj);
}
