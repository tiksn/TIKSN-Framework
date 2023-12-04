using LanguageExt;

namespace TIKSN.Integration.Correlation;

/// <summary>
///     Service for generating and parsing <see cref="CorrelationId" />.
/// </summary>
public interface ICorrelationService
{
    /// <summary>
    ///     Creates <see cref="CorrelationId" /> from string representation.
    /// </summary>
    /// <param name="stringRepresentation"></param>
    /// <returns>Created <see cref="CorrelationId"/></returns>
    CorrelationId Create(string stringRepresentation);

    /// <summary>
    ///     Creates <see cref="CorrelationId" /> from binary representation.
    /// </summary>
    /// <param name="binaryRepresentation"></param>
    /// <returns>Created <see cref="CorrelationId"/></returns>
    CorrelationId Create(Seq<byte> binaryRepresentation);

    /// <summary>
    ///     Generates new <see cref="CorrelationId" />
    /// </summary>
    /// <returns>Generated <see cref="CorrelationId"/></returns>
    CorrelationId Generate();
}
