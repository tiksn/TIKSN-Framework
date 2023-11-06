using System.Globalization;
using TIKSN.Finance;

namespace TIKSN.Globalization
{
    public interface ICurrencyFactory
    {
        CurrencyInfo Create(string isoCurrencySymbol);

        CurrencyInfo Create(RegionInfo region);
    }
}
