using LanguageExt;

namespace TIKSN.Integration.Messages;

public interface IFeatureGatedRequest
{
    public Seq<string> Features { get; }

    public bool RequireAllFeatures => true;
}
