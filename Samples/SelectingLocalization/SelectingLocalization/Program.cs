using Microsoft.Extensions.DependencyInjection;
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
			var serviceProvider = compositionRoot.CreateServiceProvider();

			var localizer = serviceProvider.GetRequiredService<IStringLocalizer>();
			var selector = serviceProvider.GetRequiredService<ILocalizationSelector>();

			var isEqual = ReferenceEquals(localizer, selector);

			Print(localizer);
			Select(selector, "ru-RU");
			Print(localizer);
			Select(selector, "ru-UA");
			Print(localizer);
			Select(selector, "ru");
			Print(localizer);
			Select(selector, "hy-AM");
			Print(localizer);
			Select(selector, "hy");
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
			Console.WriteLine(localizer.GetRequiredString(802024377));
		}
	}
}