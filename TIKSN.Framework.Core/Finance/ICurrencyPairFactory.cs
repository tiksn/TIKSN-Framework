using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using TIKSN.Globalization;

namespace TIKSN.Finance;

public interface ICurrencyPairFactory
{
    public CurrencyPair Create(CurrencyInfo baseCurrency, CurrencyInfo counterCurrency);

    public CurrencyPair Create(string baseIsoCurrencySymbol, string counterIsoCurrencySymbol);

    public CurrencyPair Create(RegionInfo baseRegion, RegionInfo counterRegion);

    public CurrencyPair Create(CountryInfo baseCountry, CountryInfo counterCountry);

    public CurrencyPair Reverse(CurrencyPair pair);

    public CurrencyPair Reverse(CurrencyInfo baseCurrency, CurrencyInfo counterCurrency);

    public CurrencyPair Reverse(string baseIsoCurrencySymbol, string counterIsoCurrencySymbol);

    public CurrencyPair Reverse(RegionInfo baseRegion, RegionInfo counterRegion);

    public CurrencyPair Reverse(CountryInfo baseCountry, CountryInfo counterCountry);

    public bool TryCreate(
        CurrencyInfo baseCurrency,
        CurrencyInfo counterCurrency,
        [NotNullWhen(true)] out CurrencyPair? pair);

    public bool TryCreate(
        string baseIsoCurrencySymbol,
        string counterIsoCurrencySymbol,
        [NotNullWhen(true)] out CurrencyPair? pair);

    public bool TryCreate(
        RegionInfo baseRegion,
        RegionInfo counterRegion,
        [NotNullWhen(true)] out CurrencyPair? pair);

    public bool TryCreate(
        CountryInfo baseCountry,
        CountryInfo counterCountry,
        [NotNullWhen(true)] out CurrencyPair? pair);

    public bool TryReverse(CurrencyPair pair, [NotNullWhen(true)] out CurrencyPair? pairReversed);

    public bool TryReverse(
        CurrencyInfo baseCurrency,
        CurrencyInfo counterCurrency,
        [NotNullWhen(true)] out CurrencyPair? pair);

    public bool TryReverse(
        string baseIsoCurrencySymbol,
        string counterIsoCurrencySymbol,
        [NotNullWhen(true)] out CurrencyPair? pair);

    public bool TryReverse(
        RegionInfo baseRegion,
        RegionInfo counterRegion,
        [NotNullWhen(true)] out CurrencyPair? pair);

    public bool TryReverse(
        CountryInfo baseCountry,
        CountryInfo counterCountry,
        [NotNullWhen(true)] out CurrencyPair? pair);
}
