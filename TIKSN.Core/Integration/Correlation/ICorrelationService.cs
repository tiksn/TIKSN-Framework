namespace TIKSN.Integration.Correlation
{
    /// <summary>
    ///     Service for generating and parsing <see cref="CorrelationID" />.
    /// </summary>
    public interface ICorrelationService
    {
        /// <summary>
        ///     Creates <see cref="CorrelationID" /> from string representation.
        /// </summary>
        /// <param name="stringRepresentation"></param>
        /// <returns></returns>
        CorrelationID Create(string stringRepresentation);

        /// <summary>
        ///     Creates <see cref="CorrelationID" /> from binary representation.
        /// </summary>
        /// <param name="byteArrayRepresentation"></param>
        /// <returns></returns>
        CorrelationID Create(byte[] byteArrayRepresentation);

        /// <summary>
        ///     Generates new <see cref="CorrelationID" />
        /// </summary>
        /// <returns></returns>
        CorrelationID Generate();
    }
}
