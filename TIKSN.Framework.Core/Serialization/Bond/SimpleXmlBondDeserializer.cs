using System.Xml;
using Bond.Protocols;

namespace TIKSN.Serialization.Bond;

public class SimpleXmlBondDeserializer : DeserializerBase<string>
{
    protected override T DeserializeInternal<T>(string serial)
    {
        using var xmlReader = XmlReader.Create(new StringReader(serial));
        var reader = new SimpleXmlReader(xmlReader);

        return global::Bond.Deserialize<T>.From(reader);
    }
}
