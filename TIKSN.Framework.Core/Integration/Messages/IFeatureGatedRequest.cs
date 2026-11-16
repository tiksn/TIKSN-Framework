using LanguageExt;

namespace TIKSN.Integration.Messages;

public sealed record FeatureFlagEnabledRequirement(string FeatureFlagKey);

public sealed record AnyFeatureFlagRequirement(Seq<FeatureFlagRequirement> Requirements);

public sealed record AllFeatureFlagsRequirement(Seq<FeatureFlagRequirement> Requirements);

#pragma warning disable CA1815
public union FeatureFlagRequirement(
FeatureFlagEnabledRequirement,
AnyFeatureFlagRequirement,
AllFeatureFlagsRequirement);
#pragma warning restore CA1815

public interface IFeatureGatedRequest
{
    public FeatureFlagRequirement FeatureRequirement { get; }
}
