using System.Collections.Generic;

namespace TIKSN.Globalization
{
    public class RegionalCurrencyRedirectionOptions
    {
        public RegionalCurrencyRedirectionOptions() =>
            this.RegionalCurrencyRedirections = new Dictionary<string, string>
(StringComparer.Ordinal)
            {
                {"001", "en-US"},
                {"150", "en-BE"},
                {"029", "en-KN"},
                {"419", "en-US"},
            };

        public Dictionary<string, string> RegionalCurrencyRedirections { get; set; }
    }
}
