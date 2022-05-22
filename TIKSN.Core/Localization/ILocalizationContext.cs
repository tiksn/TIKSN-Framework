using System.Collections.Generic;
using System.Globalization;

namespace TIKSN.Localization
{
    public interface ILocalizationContext
    {
        IReadOnlyCollection<CultureInfo> SupportedCultures { get; }
    }
}
