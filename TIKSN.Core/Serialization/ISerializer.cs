namespace TIKSN.Serialization
{
	public interface ISerializer<TSerial> where TSerial : class
	{
		TSerial Serialize(object obj);
	}
}