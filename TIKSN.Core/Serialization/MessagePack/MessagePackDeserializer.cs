using MsgPack.Serialization;
using System.IO;

namespace TIKSN.Serialization.MessagePack
{
    public class MessagePackDeserializer : DeserializerBase<byte[]>
    {
        private readonly SerializationContext _serializationContext;

        public MessagePackDeserializer(SerializationContext serializationContext)
        {
            _serializationContext = serializationContext;
        }

        protected override T DeserializeInternal<T>(byte[] serial)
        {
            var serializer = _serializationContext.GetSerializer<T>();

            using (var stream = new MemoryStream(serial))
            {
                return serializer.Unpack(stream);
            }
        }
    }
}