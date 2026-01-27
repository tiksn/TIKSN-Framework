namespace TIKSN.Configuration.ValidationStrategy;

public interface IConfigurationValidationStrategy<in T>
{
    public void Validate(T instance);
}
