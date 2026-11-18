using System.Diagnostics.CodeAnalysis;
using System.Xml.Serialization;

namespace TIKSN.Serialization;

[RequiresDynamicCode(
    "XmlSerializer generates code at runtime. Use a generated serializer for NativeAOT-compatible XML serialization.")]
[RequiresUnreferencedCode("XmlSerializer discovers members through reflection and requires preserved model types.")]
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
