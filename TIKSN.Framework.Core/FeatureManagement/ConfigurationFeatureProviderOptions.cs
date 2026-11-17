namespace TIKSN.FeatureManagement;

public class ConfigurationFeatureProviderOptions
{
    public const string DefaultSectionName = "FeatureFlags";
    public bool EnableVariantSchema { get; set; }

    public ISet<string> FalseValues { get; } = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
    {
        "false",
        "off",
        "no",
        "disabled",
        "inactive",
        "hide",
        "0",
    };

    public string SectionName { get; set; } = DefaultSectionName;

    public ISet<string> TrueValues { get; } = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
    {
        "true",
        "on",
        "yes",
        "enabled",
        "active",
        "show",
        "1",
    };
}
