using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using TIKSN.Data;
using TIKSN.Finance.ForeignExchange.Bank;
using TIKSN.Finance.ForeignExchange.Cumulative;
using TIKSN.Finance.ForeignExchange.Data;
using TIKSN.Globalization;
using TIKSN.Localization;
using TIKSN.Time;

namespace TIKSN.Finance.ForeignExchange.ExchangeRateService.IntegrationTests
{
    public sealed class TestExchangeRateService : ExchangeRateServiceBase
    {
        public TestExchangeRateService(
            ILogger<TestExchangeRateService> logger,
            IStringLocalizer stringLocalizer,
            ICurrencyFactory currencyFactory,
            IRegionFactory regionFactory,
            IExchangeRateRepository exchangeRateRepository,
            IForeignExchangeRepository foreignExchangeRepository,
            IUnitOfWorkFactory unitOfWorkFactory,
            ITimeProvider timeProvider,
            IConfiguration configuration,
            Random random) : base(logger, stringLocalizer, currencyFactory, regionFactory, exchangeRateRepository, foreignExchangeRepository, unitOfWorkFactory, random)
        {
            var currencyConverterApiKey = configuration.GetValue<string>("CurrencyConverterApiKey");

            AddBatchProvider(
                Guid.Parse("e3cabe08-7451-445b-aeec-d41824f11317"),
                new BankOfCanada(currencyFactory, timeProvider),
                LocalizationKeys.Key734344590,
                LocalizationKeys.Key734344590,
                "CA",
                TimeSpan.FromHours(12));

            AddBatchProvider(
                Guid.Parse("a007defa-4ff5-42b0-87fd-68f6cd032390"),
                new BankOfEngland(currencyFactory, regionFactory, timeProvider),
                LocalizationKeys.Key758955736,
                LocalizationKeys.Key758955736,
                "GB",
                TimeSpan.FromHours(12));

            AddBatchProvider(
                Guid.Parse("e0b68d68-5126-43a3-b0d1-4e006d6877b2"),
                new BankOfRussia(currencyFactory, timeProvider),
                LocalizationKeys.Key937230874,
                LocalizationKeys.Key466192205,
                "RU",
                TimeSpan.FromHours(24));

            AddBatchProvider(
                Guid.Parse("be2d5f28-7dc4-4f5c-a237-8de1e2f937fe"),
                new NationalBankOfUkraine(_currencyFactory),
                LocalizationKeys.Key344909065,
                LocalizationKeys.Key344909065,
                "UA",
                TimeSpan.FromHours(24));

            AddBatchProvider(
                Guid.Parse("50d993fa-0442-4376-a1ae-c27162e0222d"),
                new MyCurrencyDotNet(_currencyFactory, timeProvider),
                LocalizationKeys.Key633405189,
                LocalizationKeys.Key611756663,
                "001",
                TimeSpan.FromHours(12));

            AddIndividualProvider(
                Guid.Parse("f444836b-eba3-4766-8f54-55e83b64f969"),
                new CurrencyConverterApiDotCom(_currencyFactory, timeProvider, new CurrencyConverterApiDotCom.FreePlan(currencyConverterApiKey)),
                LocalizationKeys.Key692585112,
                LocalizationKeys.Key692585112,
                "001",
                TimeSpan.FromHours(12));
        }
    }
}
