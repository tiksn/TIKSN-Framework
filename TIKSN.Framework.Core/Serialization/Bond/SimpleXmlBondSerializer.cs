using System.Xml;
using Bond.Protocols;

namespace TIKSN.Serialization.Bond;

public class SimpleXmlBondSerializer : SerializerBase<string>
{
    protected override string SerializeInternal<T>(T obj)
    {
        using var output = new StringWriter();
        using var xmlWriter = XmlWriter.Create(output);
        var writer = new SimpleXmlWriter(xmlWriter);

        global::Bond.Serialize.To(writer, obj);

        writer.Flush();

        return output.GetStringBuilder().ToString();
    }
}
