using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using TIKSN.Data.Cache.Memory;
using TIKSN.Globalization;

namespace TIKSN.Finance;

public class CurrencyPairFactory : MemoryCacheDecoratorBase<CurrencyPair>, ICurrencyPairFactory
{
    private readonly ICurrencyFactory currencyFactory;

    public CurrencyPairFactory(
        IMemoryCache memoryCache,
        ICurrencyFactory currencyFactory,
        IOptions<MemoryCacheDecoratorOptions> genericOptions,
        IOptions<MemoryCacheDecoratorOptions<CurrencyPair>> specificOptions) : base(
        memoryCache,
        genericOptions,
        specificOptions) => this.currencyFactory =
        currencyFactory ?? throw new ArgumentNullException(nameof(currencyFactory));

    public CurrencyPair Create(CurrencyInfo baseCurrency, CurrencyInfo counterCurrency)
    {
        ArgumentNullException.ThrowIfNull(baseCurrency);
        ArgumentNullException.ThrowIfNull(counterCurrency);

        var cacheKey = Tuple.Create(
            EntityType,
            baseCurrency.ISOCurrencySymbol,
            counterCurrency.ISOCurrencySymbol);

        return this.GetFromMemoryCache(cacheKey, () => new CurrencyPair(baseCurrency, counterCurrency))
            ?? throw new InvalidOperationException("Failed to create CurrencyPair.");
    }

    public CurrencyPair Create(string baseIsoCurrencySymbol, string counterIsoCurrencySymbol)
        => this.Create(
            this.currencyFactory.Create(baseIsoCurrencySymbol),
            this.currencyFactory.Create(counterIsoCurrencySymbol));

    public CurrencyPair Create(RegionInfo baseRegion, RegionInfo counterRegion)
        => this.Create(
            this.currencyFactory.Create(baseRegion),
            this.currencyFactory.Create(counterRegion));

    public CurrencyPair Create(CountryInfo baseCountry, CountryInfo counterCountry)
    {
        ArgumentNullException.ThrowIfNull(baseCountry);
        ArgumentNullException.ThrowIfNull(counterCountry);

        return this.Create(baseCountry.PrincipalRegion, counterCountry.PrincipalRegion);
    }

    public CurrencyPair Reverse(CurrencyPair pair)
    {
        ArgumentNullException.ThrowIfNull(pair);

        return this.Create(pair.CounterCurrency, pair.BaseCurrency);
    }

    public CurrencyPair Reverse(CurrencyInfo baseCurrency, CurrencyInfo counterCurrency)
    {
        var reversedBaseCurrency = counterCurrency;
        var reversedCounterCurrency = baseCurrency;

        return this.Create(reversedBaseCurrency, reversedCounterCurrency);
    }

    public CurrencyPair Reverse(string baseIsoCurrencySymbol, string counterIsoCurrencySymbol)
    {
        var reversedBaseIsoCurrencySymbol = counterIsoCurrencySymbol;
        var reversedCounterIsoCurrencySymbol = baseIsoCurrencySymbol;

        return this.Create(reversedBaseIsoCurrencySymbol, reversedCounterIsoCurrencySymbol);
    }

    public CurrencyPair Reverse(RegionInfo baseRegion, RegionInfo counterRegion)
    {
        var reversedBaseRegion = counterRegion;
        var reversedCounterRegion = baseRegion;

        return this.Create(reversedBaseRegion, reversedCounterRegion);
    }

    public CurrencyPair Reverse(CountryInfo baseCountry, CountryInfo counterCountry)
    {
        var reversedBaseCountry = counterCountry;
        var reversedCounterCountry = baseCountry;

        return this.Create(reversedBaseCountry, reversedCounterCountry);
    }

    public bool TryCreate(
        CurrencyInfo baseCurrency,
        CurrencyInfo counterCurrency,
        [NotNullWhen(true)] out CurrencyPair? pair)
        => TryCreateCore(() => this.Create(baseCurrency, counterCurrency), out pair);

    public bool TryCreate(
        string baseIsoCurrencySymbol,
        string counterIsoCurrencySymbol,
        [NotNullWhen(true)] out CurrencyPair? pair)
        => TryCreateCore(() => this.Create(baseIsoCurrencySymbol, counterIsoCurrencySymbol), out pair);

    public bool TryCreate(
        RegionInfo baseRegion,
        RegionInfo counterRegion,
        [NotNullWhen(true)] out CurrencyPair? pair)
        => TryCreateCore(() => this.Create(baseRegion, counterRegion), out pair);

    public bool TryCreate(
        CountryInfo baseCountry,
        CountryInfo counterCountry,
        [NotNullWhen(true)] out CurrencyPair? pair)
        => TryCreateCore(() => this.Create(baseCountry, counterCountry), out pair);

    public bool TryReverse(CurrencyPair pair, [NotNullWhen(true)] out CurrencyPair? pairReversed)
        => TryCreateCore(() => this.Reverse(pair), out pairReversed);

    public bool TryReverse(
        CurrencyInfo baseCurrency,
        CurrencyInfo counterCurrency,
        [NotNullWhen(true)] out CurrencyPair? pair)
        => TryCreateCore(() => this.Reverse(baseCurrency, counterCurrency), out pair);

    public bool TryReverse(
        string baseIsoCurrencySymbol,
        string counterIsoCurrencySymbol,
        [NotNullWhen(true)] out CurrencyPair? pair)
        => TryCreateCore(() => this.Reverse(baseIsoCurrencySymbol, counterIsoCurrencySymbol), out pair);

    public bool TryReverse(
        RegionInfo baseRegion,
        RegionInfo counterRegion,
        [NotNullWhen(true)] out CurrencyPair? pair)
        => TryCreateCore(() => this.Reverse(baseRegion, counterRegion), out pair);

    public bool TryReverse(
        CountryInfo baseCountry,
        CountryInfo counterCountry,
        [NotNullWhen(true)] out CurrencyPair? pair)
        => TryCreateCore(() => this.Reverse(baseCountry, counterCountry), out pair);

    private static bool TryCreateCore(
        Func<CurrencyPair> pairFactory,
        [NotNullWhen(true)] out CurrencyPair? pair)
    {
        try
        {
            pair = pairFactory();
            return true;
        }
        catch (ArgumentException)
        {
            pair = null;
            return false;
        }
        catch (CurrencyNotFoundException)
        {
            pair = null;
            return false;
        }
    }
}
