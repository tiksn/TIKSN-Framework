using System.Globalization;
using LanguageExt;
using static LanguageExt.Prelude;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using TIKSN.Data;
using TIKSN.Finance.ForeignExchange.Data;
using TIKSN.Globalization;

namespace TIKSN.Finance.ForeignExchange
{
    public abstract class ExchangeRateServiceBase : IExchangeRateService
    {
        protected readonly ICurrencyFactory currencyFactory;
        private readonly IExchangeRateRepository exchangeRateRepository;
        private readonly IForeignExchangeRepository foreignExchangeRepository;
        protected readonly ILogger<ExchangeRateServiceBase> logger;

        private readonly Dictionary<Guid, (Either<IExchangeRateProvider, IExchangeRatesProvider> RateProvider,
            int LongNameKey, int ShortNameKey, RegionInfo Country, TimeSpan InvalidationInterval)> providers;

        private readonly Random random;
        protected readonly IRegionFactory regionFactory;
        protected readonly IStringLocalizer stringLocalizer;
        private readonly IUnitOfWorkFactory unitOfWorkFactory;

        protected ExchangeRateServiceBase(
            ILogger<ExchangeRateServiceBase> logger,
            IStringLocalizer stringLocalizer,
            ICurrencyFactory currencyFactory,
            IRegionFactory regionFactory,
            IExchangeRateRepository exchangeRateRepository,
            IForeignExchangeRepository foreignExchangeRepository,
            IUnitOfWorkFactory unitOfWorkFactory,
            Random random)
        {
            this.logger = logger;
            this.stringLocalizer = stringLocalizer;
            this.exchangeRateRepository = exchangeRateRepository;
            this.foreignExchangeRepository = foreignExchangeRepository;

            this.providers =
                new Dictionary<Guid, (Either<IExchangeRateProvider, IExchangeRatesProvider> RateProvider,
                    int LongNameKey, int ShortNameKey, RegionInfo Country, TimeSpan InvalidationInterval)>();

            this.currencyFactory = currencyFactory;
            this.regionFactory = regionFactory;
            this.random = random;
            this.unitOfWorkFactory = unitOfWorkFactory ?? throw new ArgumentNullException(nameof(unitOfWorkFactory));
        }

        public async Task<Money> ConvertCurrencyAsync(
            Money baseMoney,
            CurrencyInfo counterCurrency,
            DateTimeOffset asOn,
            CancellationToken cancellationToken)
        {
            var pair = new CurrencyPair(baseMoney.Currency, counterCurrency);

            var rate = await this.GetExchangeRateAsync(pair, asOn, cancellationToken).ConfigureAwait(false);

            return new Money(counterCurrency, baseMoney.Amount * rate);
        }

        public async Task<decimal> GetExchangeRateAsync(
            CurrencyPair pair,
            DateTimeOffset asOn,
            CancellationToken cancellationToken)
        {
            var combinedRates = new List<ExchangeRateEntity>();

            using (var uow = await this.unitOfWorkFactory.CreateAsync(cancellationToken))
            {
                foreach (var provider in this.providers)
                {
                    var ticksToIntervalRatio = asOn.Ticks / provider.Value.InvalidationInterval.Ticks;
                    var dateFrom = new DateTimeOffset(ticksToIntervalRatio * provider.Value.InvalidationInterval.Ticks,
                        asOn.Offset).UtcDateTime;
                    var dateTo =
                        new DateTimeOffset((ticksToIntervalRatio + 1) * provider.Value.InvalidationInterval.Ticks,
                            asOn.Offset).UtcDateTime;

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

                await uow.CompleteAsync(cancellationToken).ConfigureAwait(false);
            }

            var exchangeRateEntity = combinedRates
                .MinByWithTies(item => Math.Abs((item.AsOn - asOn).Ticks))
                .First();

            this.logger.LogInformation(
                347982955,
                "Exchange rate provided by Foreign Exchange with ID {ForeignExchangeID}",
                exchangeRateEntity.ForeignExchangeID);

            return exchangeRateEntity.Rate;
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
                        forex = new ForeignExchangeEntity
                        {
                            ID = provider.Key,
                            LongNameKey = provider.Value.LongNameKey,
                            ShortNameKey = provider.Value.ShortNameKey,
                            CountryCode = provider.Value.Country.Name
                        };

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
                entities.Add(new ExchangeRateEntity
                {
                    ID = Guid.NewGuid(),
                    AsOn = exchangeRate.AsOn.UtcDateTime,
                    BaseCurrencyCode = exchangeRate.Pair.BaseCurrency.ISOCurrencySymbol,
                    CounterCurrencyCode = exchangeRate.Pair.CounterCurrency.ISOCurrencySymbol,
                    ForeignExchangeID = foreignExchangeID,
                    Rate = exchangeRate.Rate
                });
            }

            await this.exchangeRateRepository.AddRangeAsync(entities, cancellationToken).ConfigureAwait(false);
        }
    }
}
