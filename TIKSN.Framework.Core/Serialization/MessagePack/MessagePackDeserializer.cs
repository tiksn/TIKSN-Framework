using MessagePack;

namespace TIKSN.Serialization.MessagePack
{
    public class MessagePackDeserializer : DeserializerBase<byte[]>
    {
        private readonly MessagePackSerializerOptions messagePackSerializerOptions;

        public MessagePackDeserializer(MessagePackSerializerOptions messagePackSerializerOptions)
            => this.messagePackSerializerOptions = messagePackSerializerOptions ?? throw new ArgumentNullException(nameof(messagePackSerializerOptions));

        protected override T DeserializeInternal<T>(byte[] serial)
            => global::MessagePack.MessagePackSerializer.Deserialize<T>(serial, this.messagePackSerializerOptions);
    }
}
