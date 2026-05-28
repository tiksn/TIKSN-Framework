using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace TIKSN.Globalization;

public interface ICountryFactory
{
    public CountryInfo Create(string name);

    public CountryInfo Create(RegionInfo region);

    public bool TryCreate(string name, [NotNullWhen(true)] out CountryInfo? country);

    public bool TryCreate(RegionInfo region, [NotNullWhen(true)] out CountryInfo? country);
}
