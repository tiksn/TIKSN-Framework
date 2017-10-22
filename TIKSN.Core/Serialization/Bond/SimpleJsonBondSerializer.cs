using Bond.Protocols;
using System.IO;
using TIKSN.Analytics.Telemetry;

namespace TIKSN.Serialization.Bond
{
    public class SimpleJsonBondSerializer : SerializerBase<string>
    {
        public SimpleJsonBondSerializer(IExceptionTelemeter exceptionTelemeter) : base(exceptionTelemeter)
        {
        }

        protected override string SerializeInternal(object obj)
        {
            using (var output = new StringWriter())
            {
                var writer = new SimpleJsonWriter(output);

                global::Bond.Serialize.To(writer, obj);

                writer.Flush();

                return output.GetStringBuilder().ToString();
            }
        }
    }
}