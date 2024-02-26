using System.Globalization;
using LanguageExt;
using Microsoft.Extensions.Logging;
using TIKSN.Data;
using TIKSN.Finance.ForeignExchange.Data;
using TIKSN.Globalization;
using static LanguageExt.Prelude;

namespace TIKSN.Finance.ForeignExchange;

public abstract partial class ExchangeRateServiceBase : IExchangeRateService
{
    private readonly IExchangeRateRepository exchangeRateRepository;
    private readonly IForeignExchangeRepository foreignExchangeRepository;

    private readonly Dictionary<Guid, (Either<IExchangeRateProvider, IExchangeRatesProvider> RateProvider,
        int LongNameKey, int ShortNameKey, RegionInfo Country, TimeSpan InvalidationInterval)> providers;

    private readonly IUnitOfWorkFactory unitOfWorkFactory;

    protected ExchangeRateServiceBase(
        ILogger<ExchangeRateServiceBase> logger,
        IRegionFactory regionFactory,
        IExchangeRateRepository exchangeRateRepository,
        IForeignExchangeRepository foreignExchangeRepository,
        IUnitOfWorkFactory unitOfWorkFactory)
    {
        this.Logger = logger ?? throw new ArgumentNullException(nameof(logger));
        this.RegionFactory = regionFactory ?? throw new ArgumentNullException(nameof(regionFactory));
        this.exchangeRateRepository = exchangeRateRepository ?? throw new ArgumentNullException(nameof(exchangeRateRepository));
        this.foreignExchangeRepository = foreignExchangeRepository ?? throw new ArgumentNullException(nameof(foreignExchangeRepository));

        this.providers = [];

        this.unitOfWorkFactory = unitOfWorkFactory ?? throw new ArgumentNullException(nameof(unitOfWorkFactory));
    }

    public ILogger<ExchangeRateServiceBase> Logger { get; }
    public IRegionFactory RegionFactory { get; }

    public async Task<Option<Money>> ConvertCurrencyAsync(
        Money baseMoney,
        CurrencyInfo counterCurrency,
        DateTimeOffset asOn,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(baseMoney);
        ArgumentNullException.ThrowIfNull(counterCurrency);

        var pair = new CurrencyPair(baseMoney.Currency, counterCurrency);

        var rate = await this.GetExchangeRateAsync(
            pair, asOn, cancellationToken).ConfigureAwait(false);

        return rate.Map(r => new Money(counterCurrency, baseMoney.Amount * r));
    }

    public async Task<Option<Money>> ConvertCurrencyAsync(
        Money baseMoney,
        CurrencyInfo counterCurrency,
        DateTimeOffset asOn,
        CurrencyInfo intermediaryCurrency,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(baseMoney);
        ArgumentNullException.ThrowIfNull(counterCurrency);
        ArgumentNullException.ThrowIfNull(intermediaryCurrency);

        var pair = new CurrencyPair(baseMoney.Currency, counterCurrency);

        var rate = await this.GetExchangeRateAsync(
            pair, asOn, intermediaryCurrency, cancellationToken).ConfigureAwait(false);

        return rate.Map(r => new Money(counterCurrency, baseMoney.Amount * r));
    }

    public async Task<Option<decimal>> GetExchangeRateAsync(
        CurrencyPair pair,
        DateTimeOffset asOn,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(pair);

        var uow = await this.unitOfWorkFactory.CreateAsync(cancellationToken).ConfigureAwait(false);
        await using (uow.ConfigureAwait(false))
        {
            var minInvalidationInterval = this.providers.Min(x => x.Value.InvalidationInterval);
            var (dateFrom, dateTo) = EstimateDateRange(asOn, minInvalidationInterval);

            var combinedRates = await this.exchangeRateRepository.SearchAsync(
                pair.BaseCurrency.ISOCurrencySymbol, pair.CounterCurrency.ISOCurrencySymbol, dateFrom, dateTo,
                cancellationToken).ConfigureAwait(false);

            if (combinedRates.Count == 0)
            {
                combinedRates = await this.FetchExchangeRatesAsync(pair, asOn, cancellationToken).ConfigureAwait(false);
            }

            await uow.CompleteAsync(cancellationToken).ConfigureAwait(false);

            var exchangeRateEntity = GetPreferredExchangeRate(asOn, combinedRates);

            _ = exchangeRateEntity.Match(
                s => LogExchangeRateByForeignExchange(this.Logger, pair, s.ForeignExchangeID),
                () => LogExchangeRateNotFound(this.Logger, pair));

            return exchangeRateEntity.Map(x => x.Rate);
        }
    }

    public async Task<Option<decimal>> GetExchangeRateAsync(
        CurrencyPair pair,
        DateTimeOffset asOn,
        CurrencyInfo intermediaryCurrency,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(pair);
        ArgumentNullException.ThrowIfNull(intermediaryCurrency);

        var syntheticCurrencyPair1 = new CurrencyPair(pair.BaseCurrency, intermediaryCurrency);
        var syntheticCurrencyPair2 = new CurrencyPair(intermediaryCurrency, pair.CounterCurrency);
        var rate1 = await this.GetExchangeRateAsync(syntheticCurrencyPair1, asOn, cancellationToken).ConfigureAwait(false);
        var rate2 = await this.GetExchangeRateAsync(syntheticCurrencyPair2, asOn, cancellationToken).ConfigureAwait(false);

        var rate =
            from r1 in rate1
            from r2 in rate2
            select r1 * r2;

        LogCompoundExchangeRate(this.Logger, rate, rate1, rate2);

        return rate;
    }

    public virtual async Task InitializeAsync(CancellationToken cancellationToken)
    {
        var uow = await this.unitOfWorkFactory.CreateAsync(cancellationToken).ConfigureAwait(false);
        await using (uow.ConfigureAwait(false))
        {
            foreach (var provider in this.providers)
            {
                var forex = await this.foreignExchangeRepository.GetOrDefaultAsync(provider.Key,
                    cancellationToken).ConfigureAwait(false);

                if (forex == null)
                {
                    forex = new ForeignExchangeEntity(
                        provider.Key,
                        provider.Value.Country.Name);

                    await this.foreignExchangeRepository.AddAsync(forex, cancellationToken).ConfigureAwait(false);
                }
            }

            await uow.CompleteAsync(cancellationToken).ConfigureAwait(false);
        }
    }

    protected void AddBatchProvider(Guid providerID, IExchangeRatesProvider provider, int longNameKey,
        int shortNameKey, string country, TimeSpan invalidationInterval) => this.providers.Add(providerID,
        (Right(provider), longNameKey, shortNameKey, this.RegionFactory.Create(country), invalidationInterval));

    protected void AddIndividualProvider(Guid providerID, IExchangeRateProvider provider, int longNameKey,
        int shortNameKey, string country, TimeSpan invalidationInterval) => this.providers.Add(providerID,
        (Left(provider), longNameKey, shortNameKey, this.RegionFactory.Create(country), invalidationInterval));

    private static (DateTime dateFrom, DateTime dateTo) EstimateDateRange(
        DateTimeOffset asOn,
        TimeSpan invalidationInterval)
    {
        var invalidationIntervalHalf = TimeSpan.FromMilliseconds(invalidationInterval.TotalMilliseconds / 2.0);
        var dateFrom = (asOn - invalidationIntervalHalf).UtcDateTime;
        var dateTo = (asOn + invalidationIntervalHalf).UtcDateTime;
        return (dateFrom, dateTo);
    }

    private static Option<ExchangeRateEntity> GetPreferredExchangeRate(
        DateTimeOffset asOn,
        IReadOnlyList<ExchangeRateEntity> combinedRates)
        => combinedRates
            .OrderBy(item => Math.Abs((item.AsOn - asOn.LocalDateTime).Ticks))
            .ToOption();

    [LoggerMessage(
        EventId = 4905787,
        Level = LogLevel.Information,
        Message = "Compound Exchange Rate is `{CompoundRate}` based on Rate1 `{Rate1}` and Rate2 `{Rate2}`")]
    private static partial void LogCompoundExchangeRate(
        ILogger logger, Option<decimal> compoundRate, Option<decimal> rate1, Option<decimal> rate2);

    [LoggerMessage(
        EventId = 4904605,
        Level = LogLevel.Information,
        Message = "Exchange rate for {CurrencyPair} provided by Foreign Exchange with ID {ForeignExchangeID}")]
    private static partial void LogExchangeRateByForeignExchange(
        ILogger logger, CurrencyPair currencyPair, Guid? foreignExchangeID);

    [LoggerMessage(
        EventId = 4904361,
        Level = LogLevel.Information,
        Message = "Exchange rate  for {CurrencyPair} is not found")]
    private static partial void LogExchangeRateNotFound(
        ILogger logger, CurrencyPair currencyPair);

    [LoggerMessage(
        EventId = 4907633,
        Level = LogLevel.Error,
        Message = "Failed to fetch exchange rate for {CurrencyPair}")]
    private static partial void LogFetchExchangeRateFailed(
        ILogger logger, Exception ex, CurrencyPair currencyPair);

    [LoggerMessage(
        EventId = 4906959,
        Level = LogLevel.Error,
        Message = "Failed to fetch exchange rates")]
    private static partial void LogFetchExchangeRatesFailed(
        ILogger logger, Exception ex);

    private async Task<IReadOnlyList<ExchangeRateEntity>> FetchExchangeRatesAsync(
        CurrencyPair pair,
        DateTimeOffset asOn,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(pair);

        var combinedRates = new List<ExchangeRateEntity>();

        foreach (var provider in this.providers)
        {
            var (dateFrom, dateTo) = EstimateDateRange(asOn, provider.Value.InvalidationInterval);

            var rates = await this.exchangeRateRepository.SearchAsync(provider.Key,
                pair.BaseCurrency.ISOCurrencySymbol, pair.CounterCurrency.ISOCurrencySymbol, dateFrom, dateTo,
                cancellationToken).ConfigureAwait(false);

            if (rates.Count == 0)
            {
                await provider.Value.RateProvider.Match(
                    batchProvider => this.FetchExchangeRatesAsync(
                        provider.Key,
                        batchProvider, asOn,
                        cancellationToken),
                    individualProvider => this.FetchExchangeRatesAsync(
                        provider.Key,
                        individualProvider,
                        pair,
                        asOn, cancellationToken)).ConfigureAwait(false);

                rates = await this.exchangeRateRepository.SearchAsync(
                    provider.Key,
                    pair.BaseCurrency.ISOCurrencySymbol,
                    pair.CounterCurrency.ISOCurrencySymbol,
                    dateFrom,
                    dateTo,
                    cancellationToken).ConfigureAwait(false);

                combinedRates.AddRange(rates);
            }
            else
            {
                combinedRates.AddRange(rates);
            }
        }

        return combinedRates;
    }

    private async Task FetchExchangeRatesAsync(
        Guid foreignExchangeID,
        IExchangeRatesProvider batchProvider,
        DateTimeOffset asOn,
        CancellationToken cancellationToken)
    {
#pragma warning disable CA1031 // Do not catch general exception types
        try
        {
            var exchangeRates = await batchProvider.GetExchangeRatesAsync(asOn, cancellationToken).ConfigureAwait(false);

            await this.SaveExchangeRatesAsync(foreignExchangeID, exchangeRates, cancellationToken).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            LogFetchExchangeRatesFailed(this.Logger, ex);
        }
#pragma warning restore CA1031 // Do not catch general exception types
    }

    private async Task FetchExchangeRatesAsync(
        Guid foreignExchangeID,
        IExchangeRateProvider individualProvider,
        CurrencyPair pair,
        DateTimeOffset asOn,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(pair);

#pragma warning disable CA1031 // Do not catch general exception types
        try
        {
            var exchangeRate = await individualProvider.GetExchangeRateAsync(
                pair.BaseCurrency,
                pair.CounterCurrency,
                asOn,
                cancellationToken).ConfigureAwait(false);

            await this.SaveExchangeRatesAsync(foreignExchangeID, [exchangeRate], cancellationToken).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            LogFetchExchangeRateFailed(this.Logger, ex, pair);
        }
#pragma warning restore CA1031 // Do not catch general exception types
    }

    private async Task SaveExchangeRatesAsync(
        Guid foreignExchangeID,
        IEnumerable<ExchangeRate> exchangeRates,
        CancellationToken cancellationToken)
    {
        var entities = new List<ExchangeRateEntity>();

        foreach (var exchangeRate in exchangeRates)
        {
            entities.Add(new ExchangeRateEntity(
                Guid.NewGuid(),
                exchangeRate.Pair.BaseCurrency.ISOCurrencySymbol,
                exchangeRate.Pair.CounterCurrency.ISOCurrencySymbol,
                foreignExchangeID,
                exchangeRate.AsOn.UtcDateTime,
                exchangeRate.Rate));
        }

        if (entities.Count > 0)
        {
            await this.exchangeRateRepository.AddRangeAsync(entities, cancellationToken).ConfigureAwait(false);
        }
    }
}
