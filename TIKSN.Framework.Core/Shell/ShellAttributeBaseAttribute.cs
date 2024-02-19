using Microsoft.Extensions.Localization;
using TIKSN.Localization;

namespace TIKSN.Shell;

public abstract class ShellAttributeBaseAttribute : Attribute
{
    private readonly int? integerNameKey;
    private readonly string stringNameKey;

    protected ShellAttributeBaseAttribute(int nameKey) => this.integerNameKey = nameKey;

    protected ShellAttributeBaseAttribute(string nameKey) => this.stringNameKey = nameKey;

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
