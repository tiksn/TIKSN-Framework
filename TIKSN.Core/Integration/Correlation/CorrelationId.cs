using System;
using System.Linq;

namespace TIKSN.Integration.Correlation
{
    /// <summary>
    ///     Correlation ID
    /// </summary>
    public readonly struct CorrelationId : IEquatable<CorrelationId>
    {
        /// <summary>
        ///     Empty Correlation ID.
        /// </summary>
        public static readonly CorrelationId Empty;

        private readonly byte[] _byteArrayRepresentation;
        private readonly string _stringRepresentation;

        internal CorrelationId(string stringRepresentation, byte[] byteArrayRepresentation)
        {
            this._stringRepresentation =
                stringRepresentation ?? throw new ArgumentNullException(nameof(stringRepresentation));
            this._byteArrayRepresentation = byteArrayRepresentation ??
                                            throw new ArgumentNullException(nameof(byteArrayRepresentation));
        }

        /// <summary>
        ///     Implicitly convert <see cref="CorrelationId" /> to byte array.
        /// </summary>
        /// <param name="correlationId"></param>
        public static implicit operator byte[](CorrelationId correlationId) => correlationId._byteArrayRepresentation;

        /// <summary>
        ///     Implicitly convert <see cref="CorrelationId" /> to string.
        /// </summary>
        /// <param name="correlationId"></param>
        public static implicit operator string(CorrelationId correlationId) => correlationId._stringRepresentation;

        /// <summary>
        ///     Indicates whether this instance and a <paramref name="other" /> are equal by their binary representation.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(CorrelationId other) =>
            this._byteArrayRepresentation.SequenceEqual(other._byteArrayRepresentation);

        /// <summary>
        ///     Indicates whether this instance and a specified object are equal.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if (obj is null or not CorrelationId)
            {
                return false;
            }

            var other = (CorrelationId)obj;

            return this.Equals(other);
        }

        /// <summary>
        ///     Returns the hash code for this instance.
        /// </summary>
        /// <returns>A 32-bit signed integer that is the hash code for this instance.</returns>
        public override int GetHashCode()
        {
            unchecked
            {
                var hash = 27;
                for (var i = 0; i < this._byteArrayRepresentation.Length && i < 4; i++)
                {
                    hash = (13 * hash) + this._byteArrayRepresentation[i].GetHashCode();
                }

                return hash;
            }
        }

        /// <summary>
        ///     Returns binary representation.
        /// </summary>
        /// <returns></returns>
        public byte[] ToByteArray() => this._byteArrayRepresentation;

        /// <summary>
        ///     Returns string representation.
        /// </summary>
        /// <returns></returns>
        public override string ToString() => this._stringRepresentation;

        public static bool operator ==(CorrelationId left, CorrelationId right) => left.Equals(right);

        public static bool operator !=(CorrelationId left, CorrelationId right) => !(left == right);
    }
}
