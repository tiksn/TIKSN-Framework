using Bond.IO.Safe;
using Bond.Protocols;

namespace TIKSN.Serialization.Bond
{
    public class SimpleBinaryBondDeserializer : DeserializerBase<byte[]>
    {
        protected override T DeserializeInternal<T>(byte[] serial)
        {
            var input = new InputBuffer(serial);
            var reader = new SimpleBinaryReader<InputBuffer>(input);

            return global::Bond.Deserialize<T>.From(reader);
        }
    }
}
