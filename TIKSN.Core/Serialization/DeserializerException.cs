using System;

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
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}