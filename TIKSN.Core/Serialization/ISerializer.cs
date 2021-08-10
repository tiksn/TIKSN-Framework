namespace TIKSN.Serialization
{
    /// <summary>
    ///     Serializer interface
    /// </summary>
    /// <typeparam name="TSerial">Type to serialize to, usually string or byte array</typeparam>
    public interface ISerializer<TSerial> where TSerial : class
    {
        /// <summary>
        ///     Serialize to <typeparamref name="TSerial" /> type
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        TSerial Serialize<T>(T obj);
    }
}
