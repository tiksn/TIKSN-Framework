namespace TIKSN.Licensing;

internal static class LicenseKeyAlgorithms
{
    internal const string Dsa = "1.2.840.10040.4.1";
#pragma warning disable S1313 // Using hardcoded IP addresses is security-sensitive
    internal const string Ed25519 = "1.3.101.112";
#pragma warning restore S1313 // Using hardcoded IP addresses is security-sensitive
    internal const string Rsa = "1.2.840.113549.1.1.1";
}
