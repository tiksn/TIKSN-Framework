namespace TIKSN.Globalization;

public class CurrencyUnionRedirectionOptions
{
    public CurrencyUnionRedirectionOptions() =>
        this.CurrencyUnionRedirections = new Dictionary<string, string>
(StringComparer.Ordinal)
        {
            {"GGP", "en-GB" /*"en-GG"*/},
            {"JEP", "en-GB" /*"en-JE"*/},
            {"IMP", "en-GB" /*"en-IM"*/},
            {"FKP", "en-GB"},
            {"GIP", "en-GB"},
            {"SHP", "en-GB"},
        };

    public Dictionary<string, string> CurrencyUnionRedirections { get; set; }
}
