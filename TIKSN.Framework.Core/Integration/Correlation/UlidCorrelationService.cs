using System.Globalization;
using LanguageExt;
using static LanguageExt.Prelude;

namespace TIKSN.Integration.Correlation;

/// <summary>
///     Service for generating and parsing <see cref="Ulid" /> backed <see cref="CorrelationId" />.
/// </summary>
public class UlidCorrelationService : ICorrelationService
{
    /// <summary>
    ///     Creates <see cref="CorrelationId" /> from string representation.
    /// </summary>
    /// <param name="stringRepresentation">String Representation</param>
    /// <returns>Created <see cref="CorrelationId"/></returns>
    public CorrelationId Create(string stringRepresentation)
    {
        var ulid = Ulid.Parse(stringRepresentation, CultureInfo.InvariantCulture);
        return new CorrelationId(ulid.ToString(), Seq(ulid.ToByteArray()));
    }

    /// <summary>
    ///     Creates <see cref="CorrelationId" /> from binary representation.
    /// </summary>
    /// <param name="binaryRepresentation">Binary Representation</param>
    /// <returns>Created <see cref="CorrelationId"/></returns>
    public CorrelationId Create(Seq<byte> binaryRepresentation)
    {
        var ulid = new Ulid(binaryRepresentation.ToArray());
        return new CorrelationId(ulid.ToString(), Seq(ulid.ToByteArray()));
    }

    /// <summary>
    ///     Generates new <see cref="CorrelationId" />
    /// </summary>
    /// <returns>Generated <see cref="CorrelationId"/></returns>
    public CorrelationId Generate()
    {
        var ulid = Ulid.NewUlid();
        return new CorrelationId(ulid.ToString(), Seq(ulid.ToByteArray()));
    }
}
