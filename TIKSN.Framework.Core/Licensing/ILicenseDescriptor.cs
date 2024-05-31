namespace TIKSN.Licensing;
#pragma warning disable S2326 // Unused type parameters should be removed

public interface ILicenseDescriptor<TEntitlements>
#pragma warning restore S2326 // Unused type parameters should be removed
{
    Guid Discriminator { get; }
    string Name { get; }
}
#pragma warning restore S2326 // Unused type parameters should be removed
