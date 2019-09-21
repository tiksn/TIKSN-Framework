using System.IO;
using System.Xml.Serialization;

namespace TIKSN.Serialization
{
    public class DotNetXmlSerializer : SerializerBase<string>
    {
        protected override string SerializeInternal(object obj)
        {
            var serializer = new XmlSerializer(obj.GetType());
            var writer = new StringWriter();
            serializer.Serialize(writer, obj);

            return writer.ToString();
        }
    }
}