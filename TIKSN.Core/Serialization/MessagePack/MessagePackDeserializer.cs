using MsgPack.Serialization;
using System.IO;
using TIKSN.Analytics.Telemetry;

namespace TIKSN.Serialization.MessagePack
{
	public class MessagePackDeserializer : DeserializerBase<byte[]>
	{
		private readonly SerializationContext _serializationContext;

		public MessagePackDeserializer(IExceptionTelemeter exceptionTelemeter, SerializationContext serializationContext) : base(exceptionTelemeter)
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
