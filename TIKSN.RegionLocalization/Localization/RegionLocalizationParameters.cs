using System.Reflection;

namespace TIKSN.Localization;

/// <summary>
/// Region Localization Parameters
/// </summary>
public static class RegionLocalizationParameters
{
    /// <summary>
    /// Gets Default Culture Name
    /// </summary>
    /// <returns>Default Culture Name</returns>
    public static string GetDefaultCultureName() =>
        typeof(RegionLocalizationParameters).GetTypeInfo().Assembly.GetName().CultureName;
}
