using Microsoft.Extensions.Configuration;
using System;
using TIKSN.Configuration;

namespace PowerShellCoreModule
{
	public class ConfigurationRootSetup : ConfigurationRootSetupBase
	{
		private static Lazy<IConfigurationRoot> lazyConfigurationRoot = new Lazy<IConfigurationRoot>(() => new ConfigurationRootSetup().CreateConfigurationRoot(), false);

		public static IConfigurationRoot ConfigurationRoot { get { return lazyConfigurationRoot.Value; } }
	}
}
