using Microsoft.Extensions.DependencyInjection;
using TIKSN.DependencyInjection;
using TIKSN.Finance;

namespace TIKSN.IntegrationTests.Finance.ForeignExchange.Bank;

internal static class CurrencyPairTestHelper
{
    public static ICurrencyPairFactory CurrencyPairFactory { get; } = CreateCurrencyPairFactory();

    private static ICurrencyPairFactory CreateCurrencyPairFactory()
    {
        var services = new ServiceCollection();
        _ = services.AddFrameworkCore();
        return services.BuildServiceProvider().GetRequiredService<ICurrencyPairFactory>();
    }
}
