namespace TIKSN.Configuration
{
	public interface IPartialConfigurationValidator<in T>
	{
		void Validate(T instance);
	}
}
