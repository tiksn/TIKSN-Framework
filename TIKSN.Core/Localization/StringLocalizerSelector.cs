using Microsoft.Extensions.Localization;
using System.Collections.Generic;
using System.Globalization;

namespace TIKSN.Localization
{
	public class StringLocalizerSelector : IStringLocalizer, ILocalizationSelector
	{
		private readonly IStringLocalizer _originalStringLocalizer;
		private IStringLocalizer _selectedStringLocalizer;

		public StringLocalizerSelector(IStringLocalizer originalStringLocalizer)
		{
			_originalStringLocalizer = originalStringLocalizer;
			_selectedStringLocalizer = originalStringLocalizer;
		}

		public LocalizedString this[string name] => _selectedStringLocalizer[name];

		public LocalizedString this[string name, params object[] arguments] => _selectedStringLocalizer[name, arguments];

		public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures)
		{
			return _selectedStringLocalizer.GetAllStrings(includeParentCultures);
		}

		public void Select(CultureInfo cultureInfo)
		{
			_selectedStringLocalizer = _originalStringLocalizer.WithCulture(cultureInfo);
		}

		public IStringLocalizer WithCulture(CultureInfo culture)
		{
			return _originalStringLocalizer.WithCulture(culture);
		}
	}
}
