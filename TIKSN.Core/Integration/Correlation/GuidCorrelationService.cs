using LanguageExt;
using static LanguageExt.Prelude;

namespace TIKSN.Integration.Correlation;

/// <summary>
///     Service for generating and parsing <see cref="Guid" /> backed <see cref="CorrelationId" />.
/// </summary>
public class GuidCorrelationService : ICorrelationService
{
    /// <summary>
    ///     Creates <see cref="CorrelationId" /> from string representation.
    /// </summary>
    /// <param name="stringRepresentation"></param>
    /// <returns></returns>
    public CorrelationId Create(string stringRepresentation)
    {
        var guid = Guid.Parse(stringRepresentation);
        return new CorrelationId(guid.ToString(), Seq(guid.ToByteArray()));
    }

    /// <summary>
    ///     Creates <see cref="CorrelationId" /> from binary representation.
    /// </summary>
    /// <param name="binaryRepresentation"></param>
    /// <returns></returns>
    public CorrelationId Create(Seq<byte> binaryRepresentation)
    {
        var guid = new Guid(binaryRepresentation.ToArray());
        return new CorrelationId(guid.ToString(), Seq(guid.ToByteArray()));
    }

    /// <summary>
    ///     Generates new <see cref="CorrelationId" />
    /// </summary>
    /// <returns></returns>
    public CorrelationId Generate()
    {
        var guid = Guid.NewGuid();
        return new CorrelationId(guid.ToString(), Seq(guid.ToByteArray()));
    }
}
