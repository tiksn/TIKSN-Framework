namespace TIKSN.Serialization
{
    /// <summary>
    ///     Custom (specialized or typed) serializer interface
    /// </summary>
    /// <typeparam name="TSerial">Type to serialize to, usually string or byte array</typeparam>
    public interface ICustomSerializer<TSerial, TModel>
    {
        /// <summary>
        ///     Serialize <typeparamref name="TModel" /> to <typeparamref name="TSerial" /> type
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        TSerial Serialize(TModel obj);
    }
}
