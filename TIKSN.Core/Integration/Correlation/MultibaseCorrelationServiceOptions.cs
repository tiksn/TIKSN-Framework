using Multiformats.Base;

namespace TIKSN.Integration.Correlation
{
    public class MultibaseCorrelationServiceOptions
    {
        public MultibaseCorrelationServiceOptions()
        {
            this.ByteLength = 16;
            this.Encoding = MultibaseEncoding.Base64;
        }

        public int ByteLength { get; set; }
        public MultibaseEncoding Encoding { get; internal set; }
    }
}
