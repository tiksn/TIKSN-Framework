using System.Xml.Serialization;

namespace TIKSN.Serialization;

public class DotNetXmlSerializer : SerializerBase<string>
{
    protected override string SerializeInternal<T>(T obj)
    {
        ArgumentNullException.ThrowIfNull(obj);

        var serializer = new XmlSerializer(obj.GetType());
        using var writer = new StringWriter();
        serializer.Serialize(writer, obj);

        return writer.ToString();
    }
}
