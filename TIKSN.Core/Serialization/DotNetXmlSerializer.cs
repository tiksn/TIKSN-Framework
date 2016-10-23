using System.IO;
using System.Xml.Serialization;
using TIKSN.Analytics.Telemetry;

namespace TIKSN.Serialization
{
    public class DotNetXmlSerializer : SerializerBase
    {
        public DotNetXmlSerializer(IExceptionTelemeter exceptionTelemeter) : base(exceptionTelemeter)
        {
        }

        protected override string SerializeInternal(object obj)
        {
            var serializer = new XmlSerializer(obj.GetType());
            var writer = new StringWriter();
            serializer.Serialize(writer, obj);

            return writer.ToString();
        }
    }
}