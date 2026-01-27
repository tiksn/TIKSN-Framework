using System.Globalization;

namespace TIKSN.Globalization;

public interface ICultureFactory
{
    public CultureInfo Create(string name);
}
