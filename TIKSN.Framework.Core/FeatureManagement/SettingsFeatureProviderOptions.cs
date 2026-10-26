namespace TIKSN.FeatureManagement;

public class SettingsFeatureProviderOptions
{
    public const string DefaultKeyPrefix = "FeatureFlags:";

    public string KeyPrefix { get; set; } = DefaultKeyPrefix;

    public SettingsLookupScope LookupScope { get; set; } = SettingsLookupScope.LocalThenRoaming;
}
