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
    /// <param name="stringRepresentation"></param>
    /// <returns></returns>
    public CorrelationId Create(string stringRepresentation)
    {
        var ulid = Ulid.Parse(stringRepresentation);
        return new CorrelationId(ulid.ToString(), Seq(ulid.ToByteArray()));
    }

    /// <summary>
    ///     Creates <see cref="CorrelationId" /> from binary representation.
    /// </summary>
    /// <param name="binaryRepresentation"></param>
    /// <returns></returns>
    public CorrelationId Create(Seq<byte> binaryRepresentation)
    {
        var ulid = new Ulid(binaryRepresentation.ToArray());
        return new CorrelationId(ulid.ToString(), Seq(ulid.ToByteArray()));
    }

    /// <summary>
    ///     Generates new <see cref="CorrelationId" />
    /// </summary>
    /// <returns></returns>
    public CorrelationId Generate()
    {
        var ulid = Ulid.NewUlid();
        return new CorrelationId(ulid.ToString(), Seq(ulid.ToByteArray()));
    }
}
