using Bond.Protocols;
using System.IO;
using TIKSN.Analytics.Telemetry;

namespace TIKSN.Serialization.Bond
{
	public class SimpleJsonBondDeserializer : DeserializerBase<string>
	{
		public SimpleJsonBondDeserializer(IExceptionTelemeter exceptionTelemeter) : base(exceptionTelemeter)
		{
		}

		protected override T DeserializeInternal<T>(string serial)
		{
			var reader = new SimpleJsonReader(new StringReader(serial));

			return global::Bond.Deserialize<T>.From(reader);
		}
	}
}
