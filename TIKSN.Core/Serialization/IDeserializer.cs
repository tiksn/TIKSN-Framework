namespace TIKSN.Serialization
{
	public interface IDeserializer<TSerial> where TSerial : class
	{
		T Deserialize<T>(TSerial serial);
	}
}