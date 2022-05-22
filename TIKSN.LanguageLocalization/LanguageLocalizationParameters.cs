using System.Reflection;

namespace TIKSN.Localization
{
    public static class LanguageLocalizationParameters
    {
        public static string GetDefaultCultureName() =>
            typeof(LanguageLocalizationParameters).GetTypeInfo().Assembly.GetName().CultureName;
    }
}
