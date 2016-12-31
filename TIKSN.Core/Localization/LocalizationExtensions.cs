using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;

namespace TIKSN.Localization
{
	public static class LocalizationExtensions
	{
		public static string GetRequiredString(this IStringLocalizer stringLocalizer, string name)
		{
			var localizedString = stringLocalizer.GetString(name);

			if (localizedString.ResourceNotFound)
				throw new KeyNotFoundException($"Resource for key '{name}' is not found.");

			return localizedString.Value;
		}

		public static string GetRequiredString(this IStringLocalizer stringLocalizer, Guid ID)
		{
			return stringLocalizer.GetRequiredString(ID.ToString());
		}

		public static string GetRequiredString(this IStringLocalizer stringLocalizer, Guid ID, params object[] arguments)
		{
			return stringLocalizer.GetRequiredString(ID.ToString(), arguments);
		}

		public static string GetRequiredString(this IStringLocalizer stringLocalizer, string name, params object[] arguments)
		{
			var format = stringLocalizer.GetRequiredString(name);

			return string.Format(format, arguments);
		}
	}
}