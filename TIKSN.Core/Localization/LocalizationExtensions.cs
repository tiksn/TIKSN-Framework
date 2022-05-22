using System;
using System.Collections.Generic;
using Microsoft.Extensions.Localization;

namespace TIKSN.Localization
{
    public static class LocalizationExtensions
    {
        public static string GetRequiredString(this IStringLocalizer stringLocalizer, string name)
        {
            var localizedString = stringLocalizer.GetString(name);

            if (localizedString.ResourceNotFound)
            {
                throw new KeyNotFoundException($"Resource for key '{name}' is not found.");
            }

            return localizedString.Value;
        }

        public static string GetRequiredString(this IStringLocalizer stringLocalizer, Guid key) =>
            stringLocalizer.GetRequiredString(key.ToString());

        public static string GetRequiredString(this IStringLocalizer stringLocalizer, int key) =>
            stringLocalizer.GetRequiredString(key.ToString());

        public static string GetRequiredString(this IStringLocalizer stringLocalizer, Guid key,
            params object[] arguments) => stringLocalizer.GetRequiredString(key.ToString(), arguments);

        public static string GetRequiredString(this IStringLocalizer stringLocalizer, int key,
            params object[] arguments) => stringLocalizer.GetRequiredString(key.ToString(), arguments);

        public static string GetRequiredString(this IStringLocalizer stringLocalizer, string name,
            params object[] arguments)
        {
            var format = stringLocalizer.GetRequiredString(name);

            return string.Format(format, arguments);
        }
    }
}
