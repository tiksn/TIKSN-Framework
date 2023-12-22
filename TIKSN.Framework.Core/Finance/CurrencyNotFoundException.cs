namespace TIKSN.Finance;

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
}
