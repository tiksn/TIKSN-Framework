using System;
using System.Net.Http;
using Microsoft.Extensions.Logging;
using TIKSN.Data;
using TIKSN.Finance.ForeignExchange.Bank;
using TIKSN.Finance.ForeignExchange.Cumulative;
using TIKSN.Finance.ForeignExchange.Data;
using TIKSN.Globalization;
using TIKSN.Localization;

namespace TIKSN.Finance.ForeignExchange.ExchangeRateService.IntegrationTests;

public sealed class TestExchangeRateService : ExchangeRateServiceBase
{
    public TestExchangeRateService(
        IBankOfCanada bankOfCanada,
        IBankOfEngland bankOfEngland,
        IBankOfRussia bankOfRussia,
        ICentralBankOfArmenia centralBankOfArmenia,
        IEuropeanCentralBank europeanCentralBank,
        IFederalReserveSystem financeReserveSystem,
        INationalBankOfPoland nationalBankOfPoland,
        INationalBankOfUkraine nationalBankOfUkraine,
        IReserveBankOfAustralia reserveBankOfAustralia,
        ISwissNationalBank swissNationalBank,
        ILogger<TestExchangeRateService> logger,
        IHttpClientFactory httpClientFactory,
        ICurrencyFactory currencyFactory,
        IRegionFactory regionFactory,
        IExchangeRateRepository exchangeRateRepository,
        IForeignExchangeRepository foreignExchangeRepository,
        IUnitOfWorkFactory unitOfWorkFactory,
        TimeProvider timeProvider)
        : base(logger, regionFactory, exchangeRateRepository, foreignExchangeRepository, unitOfWorkFactory)
    {
        this.AddBatchProvider(
            Guid.Parse("e3cabe08-7451-445b-aeec-d41824f11317"),
            bankOfCanada,
            LocalizationKeys.Key734344590,
            LocalizationKeys.Key734344590,
            "CA",
            TimeSpan.FromHours(12));

        this.AddBatchProvider(
            Guid.Parse("a007defa-4ff5-42b0-87fd-68f6cd032390"),
            bankOfEngland,
            LocalizationKeys.Key758955736,
            LocalizationKeys.Key758955736,
            "GB",
            TimeSpan.FromHours(12));

        this.AddBatchProvider(
            Guid.Parse("e0b68d68-5126-43a3-b0d1-4e006d6877b2"),
            bankOfRussia,
            LocalizationKeys.Key937230874,
            LocalizationKeys.Key466192205,
            "RU",
            TimeSpan.FromHours(24));

        this.AddBatchProvider(
            Guid.Parse("183ed360-7176-47e1-b41c-56ec426c2f5a"),
            centralBankOfArmenia,
            LocalizationKeys.Key171400643,
            LocalizationKeys.Key171400643,
            "AM",
            TimeSpan.FromHours(24));

        this.AddBatchProvider(
            Guid.Parse("01583288-eb70-4fe9-a7f4-4576c5538acb"),
            europeanCentralBank,
            LocalizationKeys.Key224205610,
            LocalizationKeys.Key224205610,
            "150",
            TimeSpan.FromHours(24));

        this.AddBatchProvider(
            Guid.Parse("cae0438f-af45-4fe1-95b5-8d13160aac89"),
            financeReserveSystem,
            LocalizationKeys.Key729270460,
            LocalizationKeys.Key724813953,
            "US",
            TimeSpan.FromHours(24));

        this.AddBatchProvider(
            Guid.Parse("cbe21065-1ead-463c-be4e-d95d30051101"),
            nationalBankOfPoland,
            LocalizationKeys.Key553783564,
            LocalizationKeys.Key553783564,
            "PL",
            TimeSpan.FromHours(24));

        this.AddBatchProvider(
            Guid.Parse("be2d5f28-7dc4-4f5c-a237-8de1e2f937fe"),
            nationalBankOfUkraine,
            LocalizationKeys.Key344909065,
            LocalizationKeys.Key344909065,
            "UA",
            TimeSpan.FromHours(24));

        this.AddBatchProvider(
            Guid.Parse("07bbe75c-dd2f-481b-80bb-e316b1cbad7b"),
            reserveBankOfAustralia,
            LocalizationKeys.Key645369586,
            LocalizationKeys.Key645369586,
            "AU",
            TimeSpan.FromHours(24));

        this.AddBatchProvider(
            Guid.Parse("3390a387-1fea-43ac-91b1-3e7ee77d85ae"),
            swissNationalBank,
            LocalizationKeys.Key748923176,
            LocalizationKeys.Key748923176,
            "CH",
            TimeSpan.FromHours(24));
    }
}
