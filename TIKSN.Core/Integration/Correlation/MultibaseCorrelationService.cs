using Microsoft.Extensions.Options;
using Multiformats.Base;
using System;

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
            _random = random ?? throw new ArgumentNullException(nameof(random));
            _multibaseCorrelationServiceOptions = multibaseCorrelationServiceOptions ?? throw new ArgumentNullException(nameof(multibaseCorrelationServiceOptions));
        }

        public CorrelationID Create(string stringRepresentation)
        {
            byte[] byteArrayRepresentation = Multibase.Decode(stringRepresentation, out MultibaseEncoding encoding);
            return new CorrelationID(stringRepresentation, byteArrayRepresentation);
        }

        public CorrelationID Create(byte[] byteArrayRepresentation)
        {
            string stringRepresentation = Multibase.Encode(_multibaseCorrelationServiceOptions.Value.Encoding, byteArrayRepresentation);
            return new CorrelationID(stringRepresentation, byteArrayRepresentation);
        }

        public CorrelationID Generate()
        {
            var byteArrayRepresentation = new byte[_multibaseCorrelationServiceOptions.Value.ByteLength];
            _random.NextBytes(byteArrayRepresentation);
            return Create(byteArrayRepresentation);
        }
    }
}