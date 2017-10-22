using Bond.IO.Safe;
using Bond.Protocols;
using TIKSN.Analytics.Telemetry;

namespace TIKSN.Serialization.Bond
{
    public class CompactBinaryBondSerializer : SerializerBase<byte[]>
    {
        public CompactBinaryBondSerializer(IExceptionTelemeter exceptionTelemeter) : base(exceptionTelemeter)
        {
        }

        protected override byte[] SerializeInternal(object obj)
        {
            var output = new OutputBuffer();
            var writer = new CompactBinaryWriter<OutputBuffer>(output);

            global::Bond.Serialize.To(writer, obj);

            return output.Data.Array;
        }
    }
}