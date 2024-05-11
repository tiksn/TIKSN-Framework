using System.Reflection;

namespace TIKSN.Localization;

/// <summary>
/// Language Localization Parameters
/// </summary>
public static class LanguageLocalizationParameters
{
    /// <summary>
    /// Gets Default Culture Name
    /// </summary>
    /// <returns>Default Culture Name</returns>
    public static string? GetDefaultCultureName() =>
        typeof(LanguageLocalizationParameters).GetTypeInfo().Assembly.GetName().CultureName;
}
