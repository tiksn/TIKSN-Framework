using LanguageExt;
using LanguageExt.Common;
using static LanguageExt.Prelude;

namespace TIKSN.Licensing;

public static class LicenseExtensions
{
    public static Validation<Error, License<TEntitlements>> Validate<TEntitlements>(
        this Validation<Error, License<TEntitlements>> licenseValidation,
        Func<License<TEntitlements>, bool> predicate,
        int failCode, string failMessage) => Validate(
            licenseValidation,
            predicate,
            Error.New(failCode, failMessage));

    public static Validation<Error, License<TEntitlements>> Validate<TEntitlements>(
        this Validation<Error, License<TEntitlements>> licenseValidation,
        Func<License<TEntitlements>, bool> predicate,
        Error fail)
    {
        ArgumentNullException.ThrowIfNull(predicate);
        ArgumentNullException.ThrowIfNull(fail);

        return licenseValidation
            .Bind(license => predicate.Invoke(license)
                    ? Success<Error, License<TEntitlements>>(license)
                    : Fail<Error, License<TEntitlements>>(fail));
    }
}
