using Newtonsoft.Json;
using TIKSN.Analytics.Telemetry;

namespace TIKSN.Serialization
{
	public class JsonDeserializer : DeserializerBase
	{
		public JsonDeserializer(IExceptionTelemeter exceptionTelemeter) : base(exceptionTelemeter)
		{
		}

		protected override T DeserializeInternal<T>(string text)
		{
			return JsonConvert.DeserializeObject<T>(text);
		}
	}
}