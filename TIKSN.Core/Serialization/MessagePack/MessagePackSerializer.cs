using MsgPack.Serialization;
using System.IO;
using TIKSN.Analytics.Telemetry;

namespace TIKSN.Serialization.MessagePack
{
	public class MessagePackSerializer : SerializerBase<byte[]>
	{
		private readonly SerializationContext _serializationContext;

		public MessagePackSerializer(IExceptionTelemeter exceptionTelemeter, SerializationContext serializationContext) : base(exceptionTelemeter)
		{
			_serializationContext = serializationContext;
		}

		protected override byte[] SerializeInternal(object obj)
		{
			var serializer = _serializationContext.GetSerializer(obj.GetType());

			using (var stream = new MemoryStream())
			{
				serializer.Pack(stream, obj);
				return stream.ToArray();
			}
		}
	}
}
