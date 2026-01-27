using System.Globalization;

namespace TIKSN.Localization;

public interface ILocalizationContext
{
    public IReadOnlyCollection<CultureInfo> SupportedCultures { get; }
}
