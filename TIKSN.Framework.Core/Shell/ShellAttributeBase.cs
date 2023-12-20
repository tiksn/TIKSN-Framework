using Microsoft.Extensions.Localization;
using TIKSN.Localization;

namespace TIKSN.Shell;

public abstract class ShellAttributeBase : Attribute
{
    private readonly int? integerNameKey;
    private readonly string stringNameKey;

    protected ShellAttributeBase(int nameKey) => this.integerNameKey = nameKey;

    protected ShellAttributeBase(string nameKey) => this.stringNameKey = nameKey;

    public string GetName(IStringLocalizer stringLocalizer)
    {
        if (this.integerNameKey.HasValue)
        {
            return stringLocalizer.GetRequiredString(this.integerNameKey.Value);
        }

        if (Guid.TryParse(this.stringNameKey, out var key))
        {
            return stringLocalizer.GetRequiredString(key);
        }

        return stringLocalizer.GetRequiredString(this.stringNameKey);
    }
}
