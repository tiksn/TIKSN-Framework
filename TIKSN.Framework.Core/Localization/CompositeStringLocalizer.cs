using Microsoft.Extensions.Localization;

namespace TIKSN.Localization;

public class CompositeStringLocalizer : IStringLocalizer
{
    public CompositeStringLocalizer(IEnumerable<IStringLocalizer> localizers) => this.Localizers = localizers;

    protected CompositeStringLocalizer()
    {
    }

    public virtual IEnumerable<IStringLocalizer> Localizers { get; }

    public LocalizedString this[string name] => this.GetLocalizedString(l => l[name]);

    public LocalizedString this[string name, params object[] arguments] =>
        this.GetLocalizedString(l => l[name, arguments]);

    public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures)
    {
        var localizedStrings = new List<LocalizedString>();

        foreach (var localizer in this.Localizers)
        {
            localizedStrings.AddRange(localizer.GetAllStrings(includeParentCultures));
        }

        return localizedStrings;
    }

    private LocalizedString GetLocalizedString(Func<IStringLocalizer, LocalizedString> singleLocalizer)
    {
        var localizedStrings = this.Localizers.Select(localizer => singleLocalizer(localizer)).ToArray();

        var localizableStrings = localizedStrings.Where(item => !item.ResourceNotFound && !string.Equals(item.Name, item.Value, StringComparison.Ordinal))
            .ToArray();

        if (localizableStrings.Length > 0)
        {
            return localizableStrings[0];
        }

        return localizedStrings[0];
    }
}
