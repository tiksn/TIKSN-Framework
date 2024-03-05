using Microsoft.Extensions.Localization;
using TIKSN.Localization;

namespace TIKSN.Shell;

public abstract class ShellAttributeBaseAttribute : Attribute
{
    protected ShellAttributeBaseAttribute(string nameKey) => this.NameKey = nameKey;

    public string NameKey { get; set; }

    public string GetName(IStringLocalizer stringLocalizer)
    {
        ArgumentNullException.ThrowIfNull(stringLocalizer);

        return stringLocalizer.GetRequiredString(this.NameKey);
    }
}
