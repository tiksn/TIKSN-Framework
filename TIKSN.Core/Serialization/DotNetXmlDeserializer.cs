using System.IO;
using System.Text;
using System.Xml.Serialization;
using TIKSN.Analytics.Telemetry;

namespace TIKSN.Serialization
{
	public class DotNetXmlDeserializer : DeserializerBase<string>
	{
		public DotNetXmlDeserializer(IExceptionTelemeter exceptionTelemeter) : base(exceptionTelemeter)
		{
		}

		protected override T DeserializeInternal<T>(string serial)
		{
			if (string.IsNullOrEmpty(serial))
			{
				return default(T);
			}

			using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(serial)))
			{
				var serializer = new XmlSerializer(typeof(T));
				return (T)serializer.Deserialize(stream);
			}
		}
	}
}