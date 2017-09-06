using Bond.Protocols;
using System.IO;
using TIKSN.Analytics.Telemetry;

namespace TIKSN.Serialization.Bond
{
	public class SimpleXmlBondSerializer : SerializerBase<byte[]>
	{
		public SimpleXmlBondSerializer(IExceptionTelemeter exceptionTelemeter) : base(exceptionTelemeter)
		{
		}

		protected override byte[] SerializeInternal(object obj)
		{
			using (var output = new MemoryStream())
			{
				var writer = new SimpleXmlWriter(output);
				global::Bond.Serialize.To(writer, obj);

				return output.ToArray();
			}
		}
	}
}
