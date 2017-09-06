using Bond.IO.Safe;
using Bond.Protocols;
using Bond;
using TIKSN.Analytics.Telemetry;

namespace TIKSN.Serialization.Bond
{
	public class CompactBinaryBondDeserializer : DeserializerBase<byte[]>
	{
		public CompactBinaryBondDeserializer(IExceptionTelemeter exceptionTelemeter) : base(exceptionTelemeter)
		{
		}

		protected override T DeserializeInternal<T>(byte[] serial)
		{
			var input = new InputBuffer(serial);
			var reader = new CompactBinaryReader<InputBuffer>(input);

			return Deserialize<T>.From(reader);
		}
	}
}
