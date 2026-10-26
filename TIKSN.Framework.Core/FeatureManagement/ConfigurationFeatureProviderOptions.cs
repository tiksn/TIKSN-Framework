namespace TIKSN.FeatureManagement;

public class ConfigurationFeatureProviderOptions
{
    public const string DefaultSectionName = "FeatureFlags";

    public string SectionName { get; set; } = DefaultSectionName;
}
