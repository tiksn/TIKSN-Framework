using System.Xml;
using System.Xml.Serialization;

namespace TIKSN.Serialization;

public class DotNetXmlDeserializer : DeserializerBase<string>
{
    protected override T DeserializeInternal<T>(string serial)
    {
        if (string.IsNullOrEmpty(serial))
        {
            return default;
        }

        using var xmlReader = XmlReader.Create(new StringReader(serial));
        var serializer = new XmlSerializer(typeof(T));
        return (T)serializer.Deserialize(xmlReader);
    }
}
