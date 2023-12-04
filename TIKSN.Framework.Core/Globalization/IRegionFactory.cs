using System.Globalization;

namespace TIKSN.Globalization;

public interface IRegionFactory
{
    RegionInfo Create(string name);
}
