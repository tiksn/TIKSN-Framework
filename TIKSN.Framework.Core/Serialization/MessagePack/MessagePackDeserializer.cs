using System.IO;
using MsgPack.Serialization;

namespace TIKSN.Serialization.MessagePack
{
    public class MessagePackDeserializer : DeserializerBase<byte[]>
    {
        private readonly SerializationContext _serializationContext;

        public MessagePackDeserializer(SerializationContext serializationContext) =>
            this._serializationContext = serializationContext;

        protected override T DeserializeInternal<T>(byte[] serial)
        {
            var serializer = this._serializationContext.GetSerializer<T>();

            using var stream = new MemoryStream(serial);
            return serializer.Unpack(stream);
        }
    }
}
