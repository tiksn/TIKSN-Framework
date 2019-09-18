using Base62;
using Microsoft.Extensions.Options;
using System;
using System.Globalization;
using System.Numerics;
using TIKSN.Serialization.Numerics;

namespace TIKSN.Integration.Correlation
{
    public class Base62CorrelationService : ICorrelationService
    {
        private readonly Base62Converter _base62Converter;
        private readonly IOptions<Base62CorrelationServiceOptions> _base62CorrelationServiceOptions;
        private readonly Random _random;
        private readonly UnsignedBigIntegerBinaryDeserializer _unsignedBigIntegerBinaryDeserializer;
        private readonly UnsignedBigIntegerBinarySerializer _unsignedBigIntegerBinarySerializer;

        public Base62CorrelationService(
            Random random,
            IOptions<Base62CorrelationServiceOptions> base62CorrelationServiceOptions,
            Base62Converter base62Converter,
            UnsignedBigIntegerBinarySerializer unsignedBigIntegerBinarySerializer,
            UnsignedBigIntegerBinaryDeserializer unsignedBigIntegerBinaryDeserializer)
        {
            _random = random ?? throw new ArgumentNullException(nameof(random));
            _base62CorrelationServiceOptions = base62CorrelationServiceOptions ?? throw new ArgumentNullException(nameof(base62CorrelationServiceOptions));
            _base62Converter = base62Converter ?? throw new ArgumentNullException(nameof(base62Converter));
            _unsignedBigIntegerBinarySerializer = unsignedBigIntegerBinarySerializer ?? throw new ArgumentNullException(nameof(unsignedBigIntegerBinarySerializer));
            _unsignedBigIntegerBinaryDeserializer = unsignedBigIntegerBinaryDeserializer ?? throw new ArgumentNullException(nameof(unsignedBigIntegerBinaryDeserializer));
        }

        public CorrelationID Create(string stringRepresentation)
        {
            var number = BigInteger.Parse(stringRepresentation, CultureInfo.InvariantCulture);
            byte[] byteArrayRepresentation = _unsignedBigIntegerBinarySerializer.Serialize(number);
            return new CorrelationID(stringRepresentation, byteArrayRepresentation);
        }

        public CorrelationID Create(byte[] byteArrayRepresentation)
        {
            _random.NextBytes(byteArrayRepresentation);
            var number = _unsignedBigIntegerBinaryDeserializer.Deserialize(byteArrayRepresentation);
            string stringRepresentation = _base62Converter.Encode(number.ToString(CultureInfo.InvariantCulture));
            return new CorrelationID(stringRepresentation, byteArrayRepresentation);
        }

        public CorrelationID Generate()
        {
            var byteArrayRepresentation = new byte[_base62CorrelationServiceOptions.Value.ByteLength];
            _random.NextBytes(byteArrayRepresentation);
            var number = _unsignedBigIntegerBinaryDeserializer.Deserialize(byteArrayRepresentation);
            string stringRepresentation = _base62Converter.Encode(number.ToString(CultureInfo.InvariantCulture));
            return new CorrelationID(stringRepresentation, byteArrayRepresentation);
        }
    }
}