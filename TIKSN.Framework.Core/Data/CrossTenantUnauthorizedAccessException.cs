namespace TIKSN.Data;

public class CrossTenantUnauthorizedAccessException : Exception
{
    public CrossTenantUnauthorizedAccessException()
    {
    }

    public CrossTenantUnauthorizedAccessException(string message) : base(message)
    {
    }

    public CrossTenantUnauthorizedAccessException(string message, Exception inner) : base(message, inner)
    {
    }
}
