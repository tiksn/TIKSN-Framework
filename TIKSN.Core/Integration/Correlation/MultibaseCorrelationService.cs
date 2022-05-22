using System;
using Microsoft.Extensions.Options;
using Multiformats.Base;

namespace TIKSN.Integration.Correlation
{
    public class MultibaseCorrelationService : ICorrelationService
    {
        private readonly IOptions<MultibaseCorrelationServiceOptions> _multibaseCorrelationServiceOptions;
        private readonly Random _random;

        public MultibaseCorrelationService(
            Random random,
            IOptions<MultibaseCorrelationServiceOptions> multibaseCorrelationServiceOptions)
        {
            this._random = random ?? throw new ArgumentNullException(nameof(random));
            this._multibaseCorrelationServiceOptions = multibaseCorrelationServiceOptions ??
                                                       throw new ArgumentNullException(
                                                           nameof(multibaseCorrelationServiceOptions));
        }

        public CorrelationID Create(string stringRepresentation)
        {
            var byteArrayRepresentation = Multibase.Decode(stringRepresentation, out MultibaseEncoding _);
            return new CorrelationID(stringRepresentation, byteArrayRepresentation);
        }

        public CorrelationID Create(byte[] byteArrayRepresentation)
        {
            var stringRepresentation = Multibase.Encode(this._multibaseCorrelationServiceOptions.Value.Encoding,
                byteArrayRepresentation);
            return new CorrelationID(stringRepresentation, byteArrayRepresentation);
        }

        public CorrelationID Generate()
        {
            var byteArrayRepresentation = new byte[this._multibaseCorrelationServiceOptions.Value.ByteLength];
            this._random.NextBytes(byteArrayRepresentation);
            return this.Create(byteArrayRepresentation);
        }
    }
}
