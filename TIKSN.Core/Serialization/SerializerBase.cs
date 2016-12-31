using System;
using TIKSN.Analytics.Telemetry;

namespace TIKSN.Serialization
{
	public abstract class SerializerBase : SerializationBase, ISerializer
	{
		public SerializerBase(IExceptionTelemeter exceptionTelemeter) : base(exceptionTelemeter)
		{
		}

		public string Serialize(object obj)
		{
			try
			{
				return SerializeInternal(obj);
			}
			catch (Exception ex)
			{
				_exceptionTelemeter.TrackException(ex);

				return null;
			}
		}

		protected abstract string SerializeInternal(object obj);
	}
}