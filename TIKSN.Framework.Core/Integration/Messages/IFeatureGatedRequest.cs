using LanguageExt;

namespace TIKSN.Integration.Messages;

public sealed record FeatureFlagEnabledRequirement(string FeatureFlagKey);

public sealed record AnyFeatureFlagRequirement(Seq<FeatureFlagRequirement> Requirements);

public sealed record AllFeatureFlagsRequirement(Seq<FeatureFlagRequirement> Requirements);

public union FeatureFlagRequirement(
FeatureFlagEnabledRequirement,
AnyFeatureFlagRequirement,
AllFeatureFlagsRequirement);

public interface IFeatureGatedRequest
{
    public FeatureFlagRequirement FeatureRequirement { get; }
}
