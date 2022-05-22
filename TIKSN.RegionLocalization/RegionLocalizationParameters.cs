using System.Reflection;

namespace TIKSN.Localization
{
    public static class RegionLocalizationParameters
    {
        public static string GetDefaultCultureName() =>
            typeof(RegionLocalizationParameters).GetTypeInfo().Assembly.GetName().CultureName;
    }
}
