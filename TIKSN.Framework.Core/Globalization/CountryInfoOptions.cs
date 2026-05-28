namespace TIKSN.Globalization;

public class CountryInfoOptions
{
    public CountryInfoOptions()
        => this.CountryRegions = new Dictionary<string, List<string>>(StringComparer.Ordinal)
        {
            { "GB", ["AI", "BM", "FK", "GB", "GG", "GI", "IM", "IO", "JE", "KY", "MS", "PN", "SH", "TC", "VG"] },
            { "FR", ["BL", "FR", "GF", "GP", "MF", "MQ", "NC", "PF", "PM", "RE", "WF", "YT"] },
            { "US", ["AS", "GU", "MP", "PR", "UM", "US", "VI"] },
            { "NL", ["AW", "BQ", "CW", "NL", "SX"] },
            { "AU", ["AU", "CC", "CX", "NF"] },
            { "NZ", ["CK", "NU", "NZ", "TK"] },
            { "CN", ["CN", "HK", "MO"] },
            { "DK", ["DK", "FO", "GL"] },
            { "FI", ["AX", "FI"] },
            { "NO", ["NO", "SJ"] },
        };

    public IDictionary<string, List<string>> CountryRegions { get; }
}
