using LanguageExt;
using OpenFeature.Model;

namespace TIKSN.FeatureManagement;

public interface ILicenseFeatureResolver<in TEntitlements>
{
    public Option<bool> ResolveBoolean(TEntitlements entitlements, string flagKey) => Option<bool>.None;

    public Option<double> ResolveDouble(TEntitlements entitlements, string flagKey) => Option<double>.None;

    public Option<int> ResolveInteger(TEntitlements entitlements, string flagKey) => Option<int>.None;

    public Option<string> ResolveString(TEntitlements entitlements, string flagKey) => Option<string>.None;

    public Option<Value> ResolveStructure(TEntitlements entitlements, string flagKey) => Option<Value>.None;
}
