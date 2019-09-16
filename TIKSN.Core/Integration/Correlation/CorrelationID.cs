using System;
using System.Linq;

namespace TIKSN.Integration.Correlation
{
    /// <summary>
    /// Correlation ID
    /// </summary>
    public struct CorrelationID : IEquatable<CorrelationID>
    {
        /// <summary>
        /// Empty Correlation ID.
        /// </summary>
        public static readonly CorrelationID Empty = new CorrelationID();

        private readonly byte[] _byteArrayRepresentation;
        private readonly string _stringRepresentation;

        internal CorrelationID(string stringRepresentation, byte[] byteArrayRepresentation)
        {
            _stringRepresentation = stringRepresentation ?? throw new ArgumentNullException(nameof(stringRepresentation));
            _byteArrayRepresentation = byteArrayRepresentation ?? throw new ArgumentNullException(nameof(byteArrayRepresentation));
        }

        public static implicit operator byte[](CorrelationID correlationId)
        {
            return correlationId._byteArrayRepresentation;
        }

        public static implicit operator string(CorrelationID correlationId)
        {
            return correlationId._stringRepresentation;
        }

        /// <summary>
        /// Indicates whether this instance and a <paramref name="other"/> are equal by their binary representation.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(CorrelationID other)
        {
            return _byteArrayRepresentation.SequenceEqual(other._byteArrayRepresentation);
        }

        /// <summary>
        /// Indicates whether this instance and a specified object are equal.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if (obj == null || !(obj is CorrelationID))
            {
                return false;
            }

            var other = (CorrelationID)obj;

            return Equals(other);
        }

        /// <summary>
        /// Returns the hash code for this instance.
        /// </summary>
        /// <returns>A 32-bit signed integer that is the hash code for this instance.</returns>
        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 27;
                for (int i = 0; i < _byteArrayRepresentation.Length && i < 4; i++)
                {
                    hash = (13 * hash) + _byteArrayRepresentation[i].GetHashCode();
                }
                return hash;
            }
        }

        /// <summary>
        /// Returns binary representation.
        /// </summary>
        /// <returns></returns>
        public byte[] ToByteArray()
        {
            return _byteArrayRepresentation;
        }

        /// <summary>
        /// Returns string representation.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return _stringRepresentation;
        }
    }
}