using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using TIKSN.Finance;

namespace TIKSN.Globalization;

public interface ICurrencyFactory
{
    public CurrencyInfo Create(string isoCurrencySymbol);

    public CurrencyInfo Create(RegionInfo region);

    public CurrencyInfo Create(CountryInfo country);

    public IReadOnlyCollection<CurrencyInfo> CreateAll(CountryInfo country);

    public bool TryCreate(string isoCurrencySymbol, [NotNullWhen(true)] out CurrencyInfo? currency);

    public bool TryCreate(RegionInfo region, [NotNullWhen(true)] out CurrencyInfo? currency);

    public bool TryCreate(CountryInfo country, [NotNullWhen(true)] out CurrencyInfo? currency);

    public bool TryCreateAll(
        CountryInfo country,
        [NotNullWhen(true)] out IReadOnlyCollection<CurrencyInfo>? currencies);
}
