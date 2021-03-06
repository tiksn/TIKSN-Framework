﻿using Serilog;
using TIKSN.Analytics.Logging.Serilog;
using Xunit.Abstractions;

namespace TIKSN.DependencyInjection.Tests
{
    public class TestSerilogLoggingSetup : SerilogLoggingSetupBase
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public TestSerilogLoggingSetup(ITestOutputHelper testOutputHelper) : base()
        {
            _testOutputHelper = testOutputHelper;
        }

        protected override void SetupSerilog()
        {
            base.SetupSerilog();

            _loggerConfiguration.WriteTo.TestOutput(_testOutputHelper);
        }
    }
}