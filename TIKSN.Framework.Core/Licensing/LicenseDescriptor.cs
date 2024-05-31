namespace TIKSN.Licensing;
#pragma warning disable S2326 // Unused type parameters should be removed

public record LicenseDescriptor<TEntitlements>(
    string Name,
    Guid Discriminator) : ILicenseDescriptor<TEntitlements>;
#pragma warning restore S2326 // Unused type parameters should be removed

