namespace TIKSN.Configuration.Validator
{
    public interface IPartialConfigurationValidator<in T>
    {
        void ValidateConfiguration(T instance);
    }
}
