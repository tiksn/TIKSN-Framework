using LanguageExt;

namespace TIKSN.FeatureManagement;

public class FeatureDisabledException : Exception
{
    public FeatureDisabledException() : base("Required feature flag is disabled.")
    {
    }

    public FeatureDisabledException(string message) : base(message)
    {
    }

    public FeatureDisabledException(string message, Exception innerException) : base(message, innerException)
    {
    }

    public FeatureDisabledException(string message, Seq<string> disabledFeatures) : base(message) =>
        this.DisabledFeatures = disabledFeatures;

    public FeatureDisabledException(string message, Seq<string> disabledFeatures, Exception innerException)
        : base(message, innerException) =>
        this.DisabledFeatures = disabledFeatures;

    public Seq<string> DisabledFeatures { get; }
}
