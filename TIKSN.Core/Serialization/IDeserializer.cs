namespace TIKSN.Serialization
{
    /// <summary>
    ///     Deserializer interface
    /// </summary>
    /// <typeparam name="TSerial">Type to deserialize from, usually string or byte array</typeparam>
    public interface IDeserializer<TSerial> where TSerial : class
    {
        /// <summary>
        ///     Deserialize from <typeparamref name="TSerial" /> type
        /// </summary>
        /// <param name="serial"></param>
        /// <returns></returns>
        T Deserialize<T>(TSerial serial);
    }
}
