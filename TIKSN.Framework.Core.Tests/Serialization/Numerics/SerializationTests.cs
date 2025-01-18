using System;
using System.Numerics;
using TIKSN.Serialization.Numerics;
using Xunit;

namespace TIKSN.Tests.Serialization.Numerics;

public class SerializationTests
{
    [Fact]
    public void DeserializeSerializeUnsignedBigInteger()
    {
        var rng = new Random();
        var serializer = new UnsignedBigIntegerBinarySerializer();
        var deserializer = new UnsignedBigIntegerBinaryDeserializer();

        for (var i = 0; i < 10; i++)
        {
            var number = BigInteger.One;
            number *= rng.Next();
            number *= rng.Next();
            number *= rng.Next();
            number *= rng.Next();

            var bytes = serializer.Serialize(number);
            var recovered = deserializer.Deserialize(bytes);
            var recoveredBytes = serializer.Serialize(recovered);
            _ = recovered.Should().Be(number);
            _ = recoveredBytes.Should().BeEquivalentTo(bytes);
        }
    }
}
