using System.Numerics;

namespace TIKSN.Serialization.Numerics;

/// <summary>
///     Custom (specialized or typed) serializer for unsigned <see cref="BigInteger" />
/// </summary>
public class UnsignedBigIntegerBinarySerializer : ICustomSerializer<byte[], BigInteger>
{
    /// <summary>
    ///     Serializes unsigned <see cref="BigInteger" /> to byte array
    /// </summary>
    /// <param name="obj">Model to serialize</param>
    /// <returns>Serialized bytes</returns>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public byte[] Serialize(BigInteger obj)
    {
        if (obj.Sign < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(obj), obj, "Big integer must be non-negative number.");
        }

        var underlyingBytes = obj.ToByteArray();
        var last = underlyingBytes[^1];
        if (last == 0b_0000_0000 && underlyingBytes.Length > 1)
        {
            return underlyingBytes.SkipLast(1).ToArray();
        }

        return underlyingBytes;
    }
}
