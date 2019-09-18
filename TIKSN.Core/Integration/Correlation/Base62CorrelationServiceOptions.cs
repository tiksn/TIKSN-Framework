namespace TIKSN.Integration.Correlation
{
    public class Base62CorrelationServiceOptions
    {
        public Base62CorrelationServiceOptions()
        {
            ByteLength = 16;
        }

        public int ByteLength { get; set; }
    }
}