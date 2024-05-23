namespace TIKSN.Serialization;

public class SerializerException : Exception
{
    public SerializerException()
    {
    }

    public SerializerException(string message) : base(message)
    {
    }

    public SerializerException(string message, Exception inner) : base(message, inner)
    {
    }
}
