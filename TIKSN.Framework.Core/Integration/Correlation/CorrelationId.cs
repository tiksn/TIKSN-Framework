using LanguageExt;

namespace TIKSN.Integration.Correlation;

/// <summary>
///     Correlation ID
/// </summary>
public readonly struct CorrelationId : IEquatable<CorrelationId>
{
    /// <summary>
    ///     Empty Correlation ID.
    /// </summary>
    public static readonly CorrelationId Empty;

    private readonly Seq<byte> binaryRepresentation;
    private readonly string stringRepresentation;

    internal CorrelationId(string stringRepresentation, Seq<byte> binaryRepresentation)
    {
        this.stringRepresentation =
            stringRepresentation ?? throw new ArgumentNullException(nameof(stringRepresentation));
        this.binaryRepresentation = binaryRepresentation;
    }

    /// <summary>
    ///     Implicitly convert <see cref="CorrelationId" /> to byte array.
    /// </summary>
    /// <param name="correlationId">Correlation Id</param>
#pragma warning disable CA2225 // Operator overloads have named alternates

    public static implicit operator Seq<byte>(CorrelationId correlationId)
#pragma warning restore CA2225 // Operator overloads have named alternates
        => correlationId.binaryRepresentation;

    /// <summary>
    ///     Implicitly convert <see cref="CorrelationId" /> to string.
    /// </summary>
    /// <param name="correlationId">Correlation Id</param>
    public static implicit operator string(CorrelationId correlationId) => correlationId.stringRepresentation;

    public static bool operator !=(CorrelationId left, CorrelationId right) => !(left == right);

    public static bool operator ==(CorrelationId left, CorrelationId right) => left.Equals(right);

    /// <summary>
    ///     Indicates whether this instance and a <paramref name="other" /> are equal by their binary representation.
    /// </summary>
    /// <param name="other">Other Correlation Id</param>
    public bool Equals(CorrelationId other) =>
        this.binaryRepresentation.SequenceEqual(other.binaryRepresentation);

    /// <summary>
    ///     Indicates whether this instance and a specified object are equal.
    /// </summary>
    /// <param name="obj">Other Correlation Id</param>
    public override bool Equals(object obj)
    {
        if (obj is not CorrelationId)
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
            for (var i = 0; i < this.binaryRepresentation.Length && i < 4; i++)
            {
                hash = (13 * hash) + this.binaryRepresentation[i].GetHashCode();
            }

            return hash;
        }
    }

    /// <summary>
    ///     Returns binary representation.
    /// </summary>
    /// <returns>Binary Representation</returns>
    public Seq<byte> ToBinary() => this.binaryRepresentation;

    /// <summary>
    ///     Returns string representation.
    /// </summary>
    public override string ToString() => this.stringRepresentation;
}
