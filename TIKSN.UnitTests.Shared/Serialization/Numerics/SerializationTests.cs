using FluentAssertions;
using System;
using System.Numerics;
using Xunit;

namespace TIKSN.Serialization.Numerics.Tests
{
    public class SerializationTests
    {
        [Fact]
        public void DeserializeSerializeUnsignedBigInteger()
        {
            var rng = new Random();
            UnsignedBigIntegerBinarySerializer serializer = new UnsignedBigIntegerBinarySerializer();
            UnsignedBigIntegerBinaryDeserializer deserializer = new UnsignedBigIntegerBinaryDeserializer();

            var number = BigInteger.One;
            number *= rng.Next();
            number *= rng.Next();
            number *= rng.Next();
            number *= rng.Next();

            var bytes = serializer.Serialize(number);
            var recovered = deserializer.Deserialize(bytes);

            recovered.Should().Be(number);
        }
    }
}