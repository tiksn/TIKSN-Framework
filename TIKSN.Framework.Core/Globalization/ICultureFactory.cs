using System.Globalization;

namespace TIKSN.Globalization;

public interface ICultureFactory
{
    CultureInfo Create(string name);
}
