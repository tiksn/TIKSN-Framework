using Microsoft.Extensions.Logging;
using Xunit.Abstractions;
using Serilog;
using TIKSN.Analytics.Logging.Serilog;

namespace TIKSN.DependencyInjection.Tests
{
	public class TestSerilogLoggingSetup : SerilogLoggingSetupBase
	{
		private readonly ITestOutputHelper _testOutputHelper;

		public TestSerilogLoggingSetup(ITestOutputHelper testOutputHelper, ILoggerFactory loggerFactory) : base(loggerFactory)
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
