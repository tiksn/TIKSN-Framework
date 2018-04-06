using System.Reflection;

namespace TIKSN.Localization
{
    public static class LanguageLocalizationParameters
    {
        public static string GetDefaultCultureName()
        {
            return typeof(LanguageLocalizationParameters).GetTypeInfo().Assembly.GetName().CultureName;
        }
    }
}