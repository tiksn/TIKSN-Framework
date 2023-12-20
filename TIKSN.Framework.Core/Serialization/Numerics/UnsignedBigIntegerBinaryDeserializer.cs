using System.Numerics;

namespace TIKSN.Serialization.Numerics;

/// <summary>
///     Custom (specialized or typed) deserializer for unsigned <see cref="BigInteger" />
/// </summary>
public class UnsignedBigIntegerBinaryDeserializer : ICustomDeserializer<byte[], BigInteger>
{
    /// <summary>
    ///     Deserializes byte array to unsigned <see cref="BigInteger" />
    /// </summary>
    /// <param name="serial">Bytes to be deserialized into model</param>
    /// <returns>Deserialized model</returns>
    /// <exception cref="ArgumentNullException"></exception>
    public BigInteger Deserialize(byte[] serial)
    {
        ArgumentNullException.ThrowIfNull(serial);

        var last = serial[^1];

        if (last < 0b_1000_0000)
        {
            return new BigInteger(serial);
        }

        return new BigInteger(serial.Concat(new byte[] { 0b_0000_0000 }).ToArray());
    }
}
