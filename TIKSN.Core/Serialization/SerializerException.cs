using System;

namespace TIKSN.Serialization
{
    [Serializable]
    public class SerializerException : Exception
    {
        public SerializerException()
        {
        }

        public SerializerException(string message) : base(message)
        {
        }

        public SerializerException(string message, Exception inner) : base(message, inner)
        {
        }

        protected SerializerException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}