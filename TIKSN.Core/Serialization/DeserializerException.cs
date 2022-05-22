using System;
using System.Runtime.Serialization;

namespace TIKSN.Serialization
{
    [Serializable]
    public class DeserializerException : Exception
    {
        public DeserializerException()
        {
        }

        public DeserializerException(string message) : base(message)
        {
        }

        public DeserializerException(string message, Exception inner) : base(message, inner)
        {
        }

        protected DeserializerException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}
