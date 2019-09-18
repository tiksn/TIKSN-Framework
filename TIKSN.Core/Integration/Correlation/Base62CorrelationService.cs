using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using TIKSN.Serialization;

namespace TIKSN.Integration.Correlation
{
    public class Base62CorrelationService : ICorrelationService
    {
        private const string Alphabet = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
        private const int Radix = 62;
        private static readonly IReadOnlyDictionary<char, int> CodeMap;

        private readonly IOptions<Base62CorrelationServiceOptions> _base62CorrelationServiceOptions;
        private readonly ICustomDeserializer<byte[], BigInteger> _bigIntegerBinaryDeserializer;
        private readonly ICustomSerializer<byte[], BigInteger> _bigIntegerBinarySerializer;
        private readonly Random _random;

        static Base62CorrelationService()
        {
            var codeMap = new Dictionary<char, int>();

            Alphabet
                .ToCharArray()
                .ForEach((x, i) => codeMap.Add(x, i));

            CodeMap = codeMap;
        }

        public Base62CorrelationService(
            Random random,
            IOptions<Base62CorrelationServiceOptions> base62CorrelationServiceOptions,
            ICustomSerializer<byte[], BigInteger> bigIntegerBinarySerializer,
            ICustomDeserializer<byte[], BigInteger> bigIntegerBinaryDeserializer)
        {
            _random = random ?? throw new ArgumentNullException(nameof(random));
            _base62CorrelationServiceOptions = base62CorrelationServiceOptions ?? throw new ArgumentNullException(nameof(base62CorrelationServiceOptions));
            _bigIntegerBinarySerializer = bigIntegerBinarySerializer ?? throw new ArgumentNullException(nameof(bigIntegerBinarySerializer));
            _bigIntegerBinaryDeserializer = bigIntegerBinaryDeserializer ?? throw new ArgumentNullException(nameof(bigIntegerBinaryDeserializer));
        }

        public CorrelationID Create(string stringRepresentation)
        {
            var number = BigInteger.Zero;

            foreach (var c in stringRepresentation)
            {
                number *= Radix;
                number += CodeMap[c];
            }
            byte[] byteArrayRepresentation = _bigIntegerBinarySerializer.Serialize(number).Reverse().ToArray();
            return new CorrelationID(stringRepresentation, byteArrayRepresentation);
        }

        public CorrelationID Create(byte[] byteArrayRepresentation)
        {
            var number = _bigIntegerBinaryDeserializer.Deserialize(byteArrayRepresentation.Reverse().ToArray());
            Stack<char> chars = new Stack<char>();

            while (number != BigInteger.Zero)
            {
                var code = (int)(number % Radix);
                chars.Push(Alphabet[code]);
                number /= Radix;
            }

            if (chars.IsEmpty())
                chars.Push('0');

            string stringRepresentation = new string(chars.ToArray());
            return new CorrelationID(stringRepresentation, byteArrayRepresentation);
        }

        public CorrelationID Generate()
        {
            var byteArrayRepresentation = new byte[_base62CorrelationServiceOptions.Value.ByteLength];
            _random.NextBytes(byteArrayRepresentation);
            return Create(byteArrayRepresentation);
        }
    }
}