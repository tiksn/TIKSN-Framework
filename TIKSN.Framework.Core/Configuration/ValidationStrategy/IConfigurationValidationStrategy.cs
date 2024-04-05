namespace TIKSN.Configuration.ValidationStrategy;

public interface IConfigurationValidationStrategy<in T>
{
    void Validate(T instance);
}
