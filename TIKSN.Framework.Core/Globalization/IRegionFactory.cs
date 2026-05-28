using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace TIKSN.Globalization;

public interface IRegionFactory
{
    public RegionInfo Create(string name);

    public bool TryCreate(string name, [NotNullWhen(true)] out RegionInfo? region);
}
