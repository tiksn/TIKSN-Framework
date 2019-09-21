using System;

namespace TIKSN.Serialization
{
    public abstract class SerializerBase<TSerial> : ISerializer<TSerial> where TSerial : class
    {
        public TSerial Serialize(object obj)
        {
            try
            {
                return SerializeInternal(obj);
            }
            catch (Exception ex)
            {
                throw new SerializerException("Serialization failed.", ex);
            }
        }

        protected abstract TSerial SerializeInternal(object obj);
    }
}