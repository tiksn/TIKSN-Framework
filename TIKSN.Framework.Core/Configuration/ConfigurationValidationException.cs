namespace TIKSN.Configuration;

public class ConfigurationValidationException : Exception
{
    public ConfigurationValidationException()
    {
    }

    public ConfigurationValidationException(string message) : base(message)
    {
    }

    public ConfigurationValidationException(string message, Exception inner) : base(message, inner)
    {
    }
}
