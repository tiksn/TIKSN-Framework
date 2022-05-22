using System;

namespace TIKSN.Integration.Correlation
{
    /// <summary>
    ///     Service for generating and parsing <see cref="Guid" /> backed <see cref="CorrelationID" />.
    /// </summary>
    public class GuidCorrelationService : ICorrelationService
    {
        /// <summary>
        ///     Creates <see cref="CorrelationID" /> from string representation.
        /// </summary>
        /// <param name="stringRepresentation"></param>
        /// <returns></returns>
        public CorrelationID Create(string stringRepresentation)
        {
            var guid = Guid.Parse(stringRepresentation);
            return new CorrelationID(guid.ToString(), guid.ToByteArray());
        }

        /// <summary>
        ///     Creates <see cref="CorrelationID" /> from binary representation.
        /// </summary>
        /// <param name="byteArrayRepresentation"></param>
        /// <returns></returns>
        public CorrelationID Create(byte[] byteArrayRepresentation)
        {
            var guid = new Guid(byteArrayRepresentation);
            return new CorrelationID(guid.ToString(), guid.ToByteArray());
        }

        /// <summary>
        ///     Generates new <see cref="CorrelationID" />
        /// </summary>
        /// <returns></returns>
        public CorrelationID Generate()
        {
            var guid = Guid.NewGuid();
            return new CorrelationID(guid.ToString(), guid.ToByteArray());
        }
    }
}
