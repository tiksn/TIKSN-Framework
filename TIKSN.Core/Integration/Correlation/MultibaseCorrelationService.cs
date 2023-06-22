using LanguageExt;
using static LanguageExt.Prelude;
using Microsoft.Extensions.Options;
using Multiformats.Base;

namespace TIKSN.Integration.Correlation;

public class MultibaseCorrelationService : ICorrelationService
{
    private readonly IOptions<MultibaseCorrelationServiceOptions> multibaseCorrelationServiceOptions;
    private readonly Random random;

    public MultibaseCorrelationService(
        Random random,
        IOptions<MultibaseCorrelationServiceOptions> multibaseCorrelationServiceOptions)
    {
        this.random = random ?? throw new ArgumentNullException(nameof(random));
        this.multibaseCorrelationServiceOptions = multibaseCorrelationServiceOptions ??
                                                   throw new ArgumentNullException(
                                                       nameof(multibaseCorrelationServiceOptions));
    }

    public CorrelationId Create(string stringRepresentation)
    {
        var binaryRepresentation = Seq(Multibase.Decode(stringRepresentation, out MultibaseEncoding _));
        return new CorrelationId(stringRepresentation, binaryRepresentation);
    }

    public CorrelationId Create(Seq<byte> binaryRepresentation)
    {
        var stringRepresentation = Multibase.Encode(this.multibaseCorrelationServiceOptions.Value.Encoding,
            binaryRepresentation.ToArray());
        return new CorrelationId(stringRepresentation, binaryRepresentation);
    }

    public CorrelationId Generate()
    {
        var byteArrayRepresentation = new byte[this.multibaseCorrelationServiceOptions.Value.ByteLength];
        this.random.NextBytes(byteArrayRepresentation);
        var binaryRepresentation = Seq(byteArrayRepresentation);
        return this.Create(binaryRepresentation);
    }
}
