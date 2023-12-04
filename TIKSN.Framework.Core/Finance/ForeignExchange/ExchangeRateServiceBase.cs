using System.Globalization;
using LanguageExt;
using Microsoft.Extensions.Logging;
using TIKSN.Data;
using TIKSN.Finance.ForeignExchange.Data;
using TIKSN.Globalization;
using static LanguageExt.Prelude;

namespace TIKSN.Finance.ForeignExchange;

public abstract class ExchangeRateServiceBase : IExchangeRateService
{
    protected readonly ILogger<ExchangeRateServiceBase> logger;
    protected readonly IRegionFactory regionFactory;
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
        this.logger = logger;
        this.exchangeRateRepository = exchangeRateRepository;
        this.foreignExchangeRepository = foreignExchangeRepository;

        this.providers =
            [];

        this.regionFactory = regionFactory;
        this.unitOfWorkFactory = unitOfWorkFactory ?? throw new ArgumentNullException(nameof(unitOfWorkFactory));
    }

    public async Task<Option<Money>> ConvertCurrencyAsync(
        Money baseMoney,
        CurrencyInfo counterCurrency,
        DateTimeOffset asOn,
        CancellationToken cancellationToken)
    {
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
        await using var uow = await this.unitOfWorkFactory.CreateAsync(cancellationToken);

        var minInvalidationInterval = this.providers.Min(x => x.Value.InvalidationInterval);
        var (dateFrom, dateTo) = EstimateDateRange(asOn, minInvalidationInterval);

        var combinedRates = await this.exchangeRateRepository.SearchAsync(
            pair.BaseCurrency.ISOCurrencySymbol, pair.CounterCurrency.ISOCurrencySymbol, dateFrom, dateTo,
            cancellationToken).ConfigureAwait(false);

        if (combinedRates.Count == 0)
        {
            combinedRates = await this.FetchExchangeRatesAsync(pair, asOn, cancellationToken);
        }

        await uow.CompleteAsync(cancellationToken).ConfigureAwait(false);

        var exchangeRateEntity = GetPreferredExchangeRate(asOn, combinedRates);

        exchangeRateEntity.Match(
            s =>
                this.logger.LogInformation(
                347982955,
                "Exchange rate for {CurrencyPair} provided by Foreign Exchange with ID {ForeignExchangeID}",
                pair, s.ForeignExchangeID),
            () =>
                this.logger.LogInformation(
                1617946673,
                "Exchange rate  for {CurrencyPair} is not found ", pair));

        return exchangeRateEntity.Map(x => x.Rate);
    }

    public async Task<Option<decimal>> GetExchangeRateAsync(
        CurrencyPair pair,
        DateTimeOffset asOn,
        CurrencyInfo intermediaryCurrency,
        CancellationToken cancellationToken)
    {
        var syntheticCurrencyPair1 = new CurrencyPair(pair.BaseCurrency, intermediaryCurrency);
        var syntheticCurrencyPair2 = new CurrencyPair(intermediaryCurrency, pair.CounterCurrency);
        var rate1 = await GetExchangeRateAsync(syntheticCurrencyPair1, asOn, cancellationToken);
        var rate2 = await GetExchangeRateAsync(syntheticCurrencyPair2, asOn, cancellationToken);

        var rate =
            from r1 in rate1
            from r2 in rate2
            select r1 * r2;

        return rate;
    }

    public async Task InitializeAsync(CancellationToken cancellationToken)
    {
        using (var uow = await this.unitOfWorkFactory.CreateAsync(cancellationToken))
        {
            foreach (var provider in this.providers)
            {
                var forex = await this.foreignExchangeRepository.GetOrDefaultAsync(provider.Key,
                    cancellationToken).ConfigureAwait(false);

                if (forex == null)
                {
                    forex = new ForeignExchangeEntity(
                        provider.Key,
                        provider.Value.Country.Name,
                        provider.Value.LongNameKey,
                        provider.Value.ShortNameKey);

                    await this.foreignExchangeRepository.AddAsync(forex, cancellationToken).ConfigureAwait(false);
                }
            }

            await uow.CompleteAsync(cancellationToken).ConfigureAwait(false);
        }
    }

    protected void AddBatchProvider(Guid providerID, IExchangeRatesProvider provider, int longNameKey,
        int shortNameKey, string country, TimeSpan invalidationInterval) => this.providers.Add(providerID,
        (Right(provider), longNameKey, shortNameKey, this.regionFactory.Create(country), invalidationInterval));

    protected void AddIndividualProvider(Guid providerID, IExchangeRateProvider provider, int longNameKey,
        int shortNameKey, string country, TimeSpan invalidationInterval) => this.providers.Add(providerID,
        (Left(provider), longNameKey, shortNameKey, this.regionFactory.Create(country), invalidationInterval));

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
    {
        var exchangeRateEntity = combinedRates
            .OrderBy(item => Math.Abs((item.AsOn - asOn).Ticks))
            .ToOption();
        return exchangeRateEntity;
    }

    private async Task<IReadOnlyList<ExchangeRateEntity>> FetchExchangeRatesAsync(
        CurrencyPair pair,
        DateTimeOffset asOn,
        CancellationToken cancellationToken)
    {
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
                    batchProvider => this.FetchExchangeRatesAsync(provider.Key, batchProvider, asOn,
                        cancellationToken),
                    individualProvider => this.FetchExchangeRatesAsync(provider.Key, individualProvider, pair,
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
        try
        {
            var exchangeRates = await batchProvider.GetExchangeRatesAsync(asOn, cancellationToken).ConfigureAwait(false);

            await this.SaveExchangeRatesAsync(foreignExchangeID, exchangeRates, cancellationToken).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            this.logger.LogError(ex, ex.Message);
        }
    }

    private async Task FetchExchangeRatesAsync(
        Guid foreignExchangeID,
        IExchangeRateProvider individualProvider,
        CurrencyPair pair,
        DateTimeOffset asOn,
        CancellationToken cancellationToken)
    {
        try
        {
            var exchangeRate = await individualProvider.GetExchangeRateAsync(pair.BaseCurrency,
                pair.CounterCurrency, asOn, cancellationToken).ConfigureAwait(false);

            await this.SaveExchangeRatesAsync(foreignExchangeID, new[] { exchangeRate }, cancellationToken).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            this.logger.LogError(ex, ex.Message);
        }
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
