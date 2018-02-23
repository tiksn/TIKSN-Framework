using System.Linq;
using System.Numerics;

namespace TIKSN.Serialization.Numerics
{
    /// <summary>
    /// Custom (specialized or typed) deserializer for unsigned <see cref="BigInteger"/>
    /// </summary>
    public class IUnsignedBigIntegerBinaryDeserializer : ICustomDeserializer<byte[], BigInteger>
    {
        /// <summary>
        /// Deserialize byte array to unsigned <see cref="BigInteger"/>
        /// </summary>
        /// <param name="serial"></param>
        /// <returns></returns>
        public BigInteger Deserialize(byte[] serial)
        {
            var last = serial[serial.Length - 1];

            if (last < 0b_1000_0000)
                return new BigInteger(serial);

            return new BigInteger(serial.Concat(new byte[] { 0b_0000_0000 }).ToArray());
        }
    }
}
