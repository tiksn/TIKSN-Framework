using Bond.Protocols;

namespace TIKSN.Serialization.Bond;

public class SimpleJsonBondDeserializer : DeserializerBase<string>
{
    protected override T DeserializeInternal<T>(string serial)
    {
        var reader = new SimpleJsonReader(new StringReader(serial));

        return global::Bond.Deserialize<T>.From(reader);
    }
}
