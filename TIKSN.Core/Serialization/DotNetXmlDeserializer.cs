using System.IO;
using System.Text;
using System.Xml.Serialization;

namespace TIKSN.Serialization
{
    public class DotNetXmlDeserializer : DeserializerBase<string>
    {
        protected override T DeserializeInternal<T>(string serial)
        {
            if (string.IsNullOrEmpty(serial))
            {
                return default;
            }

            using var stream = new MemoryStream(Encoding.UTF8.GetBytes(serial));
            var serializer = new XmlSerializer(typeof(T));
            return (T)serializer.Deserialize(stream);
        }
    }
}
