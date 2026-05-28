namespace TIKSN.Globalization;

public class DateTimeZoneLookupException : Exception
{
    public DateTimeZoneLookupException()
    {
    }

    public DateTimeZoneLookupException(string message) : base(message)
    {
    }

    public DateTimeZoneLookupException(string message, Exception inner) : base(message, inner)
    {
    }
}
