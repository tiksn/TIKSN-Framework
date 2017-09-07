using Microsoft.Extensions.Configuration;

namespace TIKSN.Configuration
{
	public abstract class ConfigurationRootSetupBase
	{
		public virtual IConfigurationRoot CreateConfigurationRoot()
		{
			var builder = new ConfigurationBuilder();

			SetupConfiguration(builder);

			return builder.Build();
		}

		protected virtual void SetupConfiguration(IConfigurationBuilder builder)
		{
			builder.AddInMemoryCollection();
		}
	}
}
