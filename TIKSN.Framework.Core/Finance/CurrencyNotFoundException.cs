namespace TIKSN.Finance;

//[System.Serializable]
public class CurrencyNotFoundException : Exception
{
    public CurrencyNotFoundException()
    {
    }

    public CurrencyNotFoundException(string message) : base(message)
    {
    }

    public CurrencyNotFoundException(string message, Exception inner) : base(message, inner)
    {
    }

    //protected CurrencyNotFoundException(
    //  System.Runtime.Serialization.SerializationInfo info,
    //  System.Runtime.Serialization.StreamingContext context) : base(info, context)
    //{ }
}
