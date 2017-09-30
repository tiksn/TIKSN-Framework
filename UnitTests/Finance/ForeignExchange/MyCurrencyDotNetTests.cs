using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Threading.Tasks;
using TIKSN.DependencyInjection.Tests;
using TIKSN.Globalization;
using Xunit;
using Xunit.Abstractions;

namespace TIKSN.Finance.ForeignExchange.Tests
{
	public class MyCurrencyDotNetTests
	{
		private readonly IServiceProvider _serviceProvider;

		public MyCurrencyDotNetTests(ITestOutputHelper testOutputHelper)
		{
			_serviceProvider = new TestCompositionRootSetup(testOutputHelper, services =>
			{
				services.AddSingleton<ICurrencyFactory, CurrencyFactory>();
				services.AddSingleton<IRegionFactory, RegionFactory>();
			}).CreateServiceProvider();
		}

		[Fact]
		public async Task GetCurrencyPairsAsync()
		{
			var myCurrencyDotNet = new MyCurrencyDotNet(_serviceProvider.GetRequiredService<ICurrencyFactory>(), _serviceProvider.GetRequiredService<IRegionFactory>());

			var pairs = await myCurrencyDotNet.GetCurrencyPairsAsync(DateTimeOffset.Now);

			pairs.Count().Should().BeGreaterThan(0);
		}
	}
}
