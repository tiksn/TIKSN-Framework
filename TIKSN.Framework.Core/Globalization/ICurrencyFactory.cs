using System.Globalization;
using TIKSN.Finance;

namespace TIKSN.Globalization;

public interface ICurrencyFactory
{
    public CurrencyInfo Create(string isoCurrencySymbol);

    public CurrencyInfo Create(RegionInfo region);
}
