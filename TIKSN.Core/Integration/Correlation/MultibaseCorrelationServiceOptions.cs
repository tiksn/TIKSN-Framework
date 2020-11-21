using Multiformats.Base;

namespace TIKSN.Integration.Correlation
{
    public class MultibaseCorrelationServiceOptions
    {
        public MultibaseCorrelationServiceOptions()
        {
            ByteLength = 16;
            Encoding = MultibaseEncoding.Base64;
        }

        public int ByteLength { get; set; }
        public MultibaseEncoding Encoding { get; internal set; }
    }
}