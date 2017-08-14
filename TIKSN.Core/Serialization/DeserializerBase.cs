using System;
using TIKSN.Analytics.Telemetry;

namespace TIKSN.Serialization
{
	public abstract class DeserializerBase<TSerial> : SerializationBase, IDeserializer<TSerial> where TSerial : class
	{
		protected DeserializerBase(IExceptionTelemeter exceptionTelemeter) : base(exceptionTelemeter)
		{
		}

		public T Deserialize<T>(TSerial serial)
		{
			try
			{
				return DeserializeInternal<T>(serial);
			}
			catch (Exception ex)
			{
				_exceptionTelemeter.TrackException(ex);

				return default(T);
			}
		}

		protected abstract T DeserializeInternal<T>(TSerial serial);
	}
}