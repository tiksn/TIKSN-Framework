using MessagePack;

namespace TIKSN.Serialization.MessagePack
{
    public class MessagePackSerializer : SerializerBase<byte[]>
    {
        private readonly MessagePackSerializerOptions messagePackSerializerOptions;

        public MessagePackSerializer(MessagePackSerializerOptions messagePackSerializerOptions)
            => this.messagePackSerializerOptions = messagePackSerializerOptions ?? throw new ArgumentNullException(nameof(messagePackSerializerOptions));

        protected override byte[] SerializeInternal<T>(T obj)
            => global::MessagePack.MessagePackSerializer.Serialize(obj, this.messagePackSerializerOptions);
    }
}
