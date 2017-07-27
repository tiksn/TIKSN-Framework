using System.Globalization;

namespace TIKSN.Localization
{
	public interface ILocalizationSelector
	{
		void Select(CultureInfo cultureInfo);
		void Unselect();
	}
}
