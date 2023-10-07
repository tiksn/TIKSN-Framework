namespace TIKSN.Licensing;

public class LicenseTerms
{
    public LicenseTerms(
        Ulid serialNumber,
        Party licensor,
        Party licensee,
        DateTimeOffset notBefore,
        DateTimeOffset notAfter)
    {
        this.SerialNumber = serialNumber;
        this.Licensor = licensor;
        this.Licensee = licensee;
        this.NotBefore = notBefore;
        this.NotAfter = notAfter;
    }

    public Party Licensee { get; }
    public Party Licensor { get; }
    public DateTimeOffset NotAfter { get; }
    public DateTimeOffset NotBefore { get; }
    public Ulid SerialNumber { get; }
}
