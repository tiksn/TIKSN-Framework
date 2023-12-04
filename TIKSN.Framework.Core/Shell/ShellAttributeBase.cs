using Microsoft.Extensions.Localization;
using TIKSN.Localization;

namespace TIKSN.Shell;

public abstract class ShellAttributeBase : Attribute
{
    private readonly int? _integerNameKey;
    private readonly string _stringNameKey;

    protected ShellAttributeBase(int nameKey) => this._integerNameKey = nameKey;

    protected ShellAttributeBase(string nameKey) => this._stringNameKey = nameKey;

    public string GetName(IStringLocalizer stringLocalizer)
    {
        if (this._integerNameKey.HasValue)
        {
            return stringLocalizer.GetRequiredString(this._integerNameKey.Value);
        }

        if (Guid.TryParse(this._stringNameKey, out var key))
        {
            return stringLocalizer.GetRequiredString(key);
        }

        return stringLocalizer.GetRequiredString(this._stringNameKey);
    }
}
