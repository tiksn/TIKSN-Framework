﻿using System.Reflection;

namespace TIKSN.RegionLocalization
{
	public static class RegionLocalizationParameters
	{
		public static string GetDefaultCultureName()
		{
			return typeof(RegionLocalizationParameters).GetTypeInfo().Assembly.GetName().CultureName;
		}
	}
}
