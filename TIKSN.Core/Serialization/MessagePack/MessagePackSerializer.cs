using MsgPack.Serialization;
using System.IO;

namespace TIKSN.Serialization.MessagePack
{
    public class MessagePackSerializer : SerializerBase<byte[]>
    {
        private readonly SerializationContext _serializationContext;

        public MessagePackSerializer(SerializationContext serializationContext)
        {
            _serializationContext = serializationContext;
        }

        protected override byte[] SerializeInternal<T>(T obj)
        {
            var serializer = _serializationContext.GetSerializer<T>();

            using (var stream = new MemoryStream())
            {
                serializer.Pack(stream, obj);
                return stream.ToArray();
            }
        }
    }
}