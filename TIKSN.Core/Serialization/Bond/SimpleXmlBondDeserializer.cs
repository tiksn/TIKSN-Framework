using Bond.Protocols;
using System.IO;
using System.Xml;
using TIKSN.Analytics.Telemetry;

namespace TIKSN.Serialization.Bond
{
    public class SimpleXmlBondDeserializer : DeserializerBase<string>
    {
        public SimpleXmlBondDeserializer(IExceptionTelemeter exceptionTelemeter) : base(exceptionTelemeter)
        {
        }

        protected override T DeserializeInternal<T>(string serial)
        {
            var reader = new SimpleXmlReader(XmlReader.Create(new StringReader(serial)));

            return global::Bond.Deserialize<T>.From(reader);
        }
    }
}