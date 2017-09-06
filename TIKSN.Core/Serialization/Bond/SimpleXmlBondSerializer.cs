using Bond.Protocols;
using System.IO;
using System.Xml;
using TIKSN.Analytics.Telemetry;

namespace TIKSN.Serialization.Bond
{
	public class SimpleXmlBondSerializer : SerializerBase<string>
	{
		public SimpleXmlBondSerializer(IExceptionTelemeter exceptionTelemeter) : base(exceptionTelemeter)
		{
		}

		protected override string SerializeInternal(object obj)
		{
			using (var output = new StringWriter())
			{
				var writer = new SimpleXmlWriter(XmlWriter.Create(output));
				global::Bond.Serialize.To(writer, obj);

				writer.Flush();

				return output.GetStringBuilder().ToString();
			}
		}
	}
}
