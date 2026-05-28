using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace TIKSN.Globalization;

public interface ICultureFactory
{
    public CultureInfo Create(string name);

    public bool TryCreate(string name, [NotNullWhen(true)] out CultureInfo? culture);
}
