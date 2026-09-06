namespace TIKSN.Data;

public class CrossTenantOutboundUnauthorizedAccessException : CrossTenantUnauthorizedAccessException
{
    public CrossTenantOutboundUnauthorizedAccessException()
    {
    }

    public CrossTenantOutboundUnauthorizedAccessException(string message) : base(message)
    {
    }

    public CrossTenantOutboundUnauthorizedAccessException(string message, Exception inner) : base(message, inner)
    {
    }
}
