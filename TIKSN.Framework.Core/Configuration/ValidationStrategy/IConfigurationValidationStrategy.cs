namespace TIKSN.Configuration.ValidationStrategy
{
    public interface IConfigurationValidationStrategy<T>
    {
        void Validate(T instance);
    }
}
