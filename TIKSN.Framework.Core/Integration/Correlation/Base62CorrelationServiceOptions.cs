namespace TIKSN.Integration.Correlation;

public class Base62CorrelationServiceOptions
{
    public Base62CorrelationServiceOptions() => this.ByteLength = 16;

    public int ByteLength { get; set; }
}
