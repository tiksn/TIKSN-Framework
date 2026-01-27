using System.Globalization;

namespace TIKSN.Globalization;

public interface IRegionFactory
{
    public RegionInfo Create(string name);
}
