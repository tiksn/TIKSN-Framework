namespace TIKSN.Configuration.Validator;

public interface IPartialConfigurationValidator<in T>
{
    public void ValidateConfiguration(T instance);
}
