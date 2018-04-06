using Bond.IO.Safe;
using Bond.Protocols;
using TIKSN.Analytics.Telemetry;

namespace TIKSN.Serialization.Bond
{
    public class SimpleBinaryBondDeserializer : DeserializerBase<byte[]>
    {
        public SimpleBinaryBondDeserializer(IExceptionTelemeter exceptionTelemeter) : base(exceptionTelemeter)
        {
        }

        protected override T DeserializeInternal<T>(byte[] serial)
        {
            var input = new InputBuffer(serial);
            var reader = new SimpleBinaryReader<InputBuffer>(input);

            return global::Bond.Deserialize<T>.From(reader);
        }
    }
}