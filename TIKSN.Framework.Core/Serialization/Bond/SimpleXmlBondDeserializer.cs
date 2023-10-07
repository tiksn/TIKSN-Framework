using System.IO;
using System.Xml;
using Bond.Protocols;

namespace TIKSN.Serialization.Bond
{
    public class SimpleXmlBondDeserializer : DeserializerBase<string>
    {
        protected override T DeserializeInternal<T>(string serial)
        {
            var reader = new SimpleXmlReader(XmlReader.Create(new StringReader(serial)));

            return global::Bond.Deserialize<T>.From(reader);
        }
    }
}
