using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Xunit.Abstractions;
using TIKSN.Analytics.Logging;

namespace TIKSN.DependencyInjection.Tests
{
	public class TestCompositionRootSetup : CompositionRootSetupBase
	{
		private readonly ITestOutputHelper _testOutputHelper;
		private readonly Action<IServiceCollection> _configureServices;
		private readonly Action<IServiceCollection, IConfigurationRoot> _configureOptions;

		public TestCompositionRootSetup(ITestOutputHelper testOutputHelper, Action<IServiceCollection> configureServices = null, Action<IServiceCollection, IConfigurationRoot> configureOptions = null) : base(new TestConfigurationRootSetup().CreateConfigurationRoot())
		{
			_testOutputHelper = testOutputHelper;
			_configureServices = configureServices;
			_configureOptions = configureOptions;
		}

		protected override void ConfigureOptions(IServiceCollection services, IConfigurationRoot configuration)
		{
			_configureOptions?.Invoke(services, configuration);
		}

		protected override void ConfigureServices(IServiceCollection services)
		{
			services.AddSingleton(_testOutputHelper);
			services.AddSingleton<LoggingSetupBase, TestSerilogLoggingSetup>();
			_configureServices?.Invoke(services);
		}
	}
}
