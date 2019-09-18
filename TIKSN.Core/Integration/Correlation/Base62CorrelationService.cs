using Base62;
using Microsoft.Extensions.Options;
using System;
using System.Globalization;
using System.Numerics;
using TIKSN.Serialization;

namespace TIKSN.Integration.Correlation
{
    public class Base62CorrelationService : ICorrelationService
    {
        private readonly Base62Converter _base62Converter;
        private readonly IOptions<Base62CorrelationServiceOptions> _base62CorrelationServiceOptions;
        private readonly ICustomDeserializer<byte[], BigInteger> _bigIntegerBinaryDeserializer;
        private readonly ICustomSerializer<byte[], BigInteger> _bigIntegerBinarySerializer;
        private readonly Random _random;

        public Base62CorrelationService(
            Random random,
            IOptions<Base62CorrelationServiceOptions> base62CorrelationServiceOptions,
            Base62Converter base62Converter,
            ICustomSerializer<byte[], BigInteger> bigIntegerBinarySerializer,
            ICustomDeserializer<byte[], BigInteger> bigIntegerBinaryDeserializer)
        {
            _random = random ?? throw new ArgumentNullException(nameof(random));
            _base62CorrelationServiceOptions = base62CorrelationServiceOptions ?? throw new ArgumentNullException(nameof(base62CorrelationServiceOptions));
            _base62Converter = base62Converter ?? throw new ArgumentNullException(nameof(base62Converter));
            _bigIntegerBinarySerializer = bigIntegerBinarySerializer ?? throw new ArgumentNullException(nameof(bigIntegerBinarySerializer));
            _bigIntegerBinaryDeserializer = bigIntegerBinaryDeserializer ?? throw new ArgumentNullException(nameof(bigIntegerBinaryDeserializer));
        }

        public CorrelationID Create(string stringRepresentation)
        {
            var numberToParse = _base62Converter.Decode(stringRepresentation);
            var number = BigInteger.Parse(numberToParse, CultureInfo.InvariantCulture);
            byte[] byteArrayRepresentation = _bigIntegerBinarySerializer.Serialize(number);
            return new CorrelationID(stringRepresentation, byteArrayRepresentation);
        }

        public CorrelationID Create(byte[] byteArrayRepresentation)
        {
            _random.NextBytes(byteArrayRepresentation);
            var number = _bigIntegerBinaryDeserializer.Deserialize(byteArrayRepresentation);
            string stringRepresentation = _base62Converter.Encode(number.ToString(CultureInfo.InvariantCulture));
            return new CorrelationID(stringRepresentation, byteArrayRepresentation);
        }

        public CorrelationID Generate()
        {
            var byteArrayRepresentation = new byte[_base62CorrelationServiceOptions.Value.ByteLength];
            _random.NextBytes(byteArrayRepresentation);
            var number = _bigIntegerBinaryDeserializer.Deserialize(byteArrayRepresentation);
            string stringRepresentation = _base62Converter.Encode(number.ToString(CultureInfo.InvariantCulture));
            return new CorrelationID(stringRepresentation, byteArrayRepresentation);
        }
    }
}