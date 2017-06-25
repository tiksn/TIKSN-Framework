using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace TIKSN.Localization
{
	public class CompositeStringLocalizer : IStringLocalizer
	{
		public CompositeStringLocalizer(IEnumerable<IStringLocalizer> localizers)
		{
			Localizers = localizers;
		}

		protected CompositeStringLocalizer()
		{
		}

		public virtual IEnumerable<IStringLocalizer> Localizers { get; }

		public LocalizedString this[string name]
		{
			get
			{
				return GetLocalizedString(l => l[name]);
			}
		}

		public LocalizedString this[string name, params object[] arguments]
		{
			get
			{
				return GetLocalizedString(l => l[name, arguments]);
			}
		}

		public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures)
		{
			var localizedStrings = new List<LocalizedString>();

			foreach (var localizer in Localizers)
			{
				localizedStrings.AddRange(localizer.GetAllStrings(includeParentCultures));
			}

			return localizedStrings;
		}

		public IStringLocalizer WithCulture(CultureInfo culture)
		{
			return new CompositeStringLocalizer(Localizers.Select(item => item.WithCulture(culture)));
		}

		private LocalizedString GetLocalizedString(Func<IStringLocalizer, LocalizedString> singleLocalizer)
		{
			var localizedStrings = new List<LocalizedString>();

			foreach (var localizer in Localizers)
			{
				localizedStrings.Add(singleLocalizer(localizer));
			}

			if (localizedStrings.Any(item => !item.ResourceNotFound))
				return localizedStrings.Single(item => !item.ResourceNotFound);

			return localizedStrings.First();
		}
	}
}