using System;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TIKSN.Analytics.Logging;
using Xunit.Abstractions;

namespace TIKSN.DependencyInjection.Tests
{
    public class TestCompositionRootSetup : CompositionRootSetupBase
    {
        private readonly ITestOutputHelper _testOutputHelper;
        private readonly Action<IServiceCollection> _configureServices;
        private readonly Action<IServiceCollection, IConfigurationRoot> _configureOptions;

        public TestCompositionRootSetup(ITestOutputHelper testOutputHelper, Action<IServiceCollection> configureServices = null, Action<IServiceCollection, IConfigurationRoot> configureOptions = null) : base(new TestConfigurationRootSetup().GetConfigurationRoot())
        {
            this._testOutputHelper = testOutputHelper;
            this._configureServices = configureServices;
            this._configureOptions = configureOptions;
        }

        protected override void ConfigureOptions(IServiceCollection services, IConfigurationRoot configuration) => this._configureOptions?.Invoke(services, configuration);

        protected override void ConfigureServices(IServiceCollection services) => this._configureServices?.Invoke(services);

        protected override IEnumerable<ILoggingSetup> GetLoggingSetups()
        {
            yield return new TestSerilogLoggingSetup(this._testOutputHelper);
        }
    }
}
