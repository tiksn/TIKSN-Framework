using Serilog;
using TIKSN.Analytics.Logging.Serilog;
using Xunit.Abstractions;

namespace TIKSN.DependencyInjection.Tests
{
    public class TestSerilogLoggingSetup : SerilogLoggingSetupBase
    {
        private readonly ITestOutputHelper testOutputHelper;

        public TestSerilogLoggingSetup(ITestOutputHelper testOutputHelper) : base() => this.testOutputHelper = testOutputHelper;

        protected override void SetupSerilog()
        {
            base.SetupSerilog();

            _ = this.LoggerConfiguration.WriteTo.TestOutput(this.testOutputHelper);
        }
    }
}
