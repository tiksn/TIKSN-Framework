using Bond.IO.Safe;
using Bond.Protocols;

namespace TIKSN.Serialization.Bond;

public class FastBinaryBondSerializer : SerializerBase<byte[]>
{
    protected override byte[] SerializeInternal<T>(T obj)
    {
        var output = new OutputBuffer();
        var writer = new FastBinaryWriter<OutputBuffer>(output);

        global::Bond.Serialize.To(writer, obj);

        return output.Data.Array;
    }
}
