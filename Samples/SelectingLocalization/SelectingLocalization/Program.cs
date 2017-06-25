﻿using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using System;
using System.Globalization;
using TIKSN.Localization;

namespace SelectingLocalization
{
	class Program
	{
		static void Main(string[] args)
		{
			var compositionRoot = new CompositionRoot();
			var serviceProvider =  compositionRoot.CreateServiceProvider();

			var localizer = serviceProvider.GetRequiredService<IStringLocalizer>();
			var selector = serviceProvider.GetRequiredService<ILocalizationSelector>();

			
			Print(localizer);
			Select(selector, "ru-RU");
			Print(localizer);
			Select(selector, "hy-AM");
			Print(localizer);

			Console.ReadLine();
		}

		private static void Select(ILocalizationSelector selector, string name)
		{
			selector.Select(new CultureInfo(name));
			Console.WriteLine($"Culture '{name}' is selected.");
		}

		private static void Print(IStringLocalizer localizer)
		{
			Console.WriteLine(localizer.GetRequiredString("066c2ac3-cacc-4271-9ed8-8a5cf9fb8369"));
		}
	}
}