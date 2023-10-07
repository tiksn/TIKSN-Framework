using System.Net.Mail;

namespace TIKSN.Licensing;

public sealed class OrganizationParty : Party
{
    public OrganizationParty(
        string longName,
        string shortName,
        MailAddress email,
        Uri website) : base(
            email,
            website)
    {
        this.LongName = longName;
        this.ShortName = shortName;
    }

    public string LongName { get; }
    public string ShortName { get; }
}
