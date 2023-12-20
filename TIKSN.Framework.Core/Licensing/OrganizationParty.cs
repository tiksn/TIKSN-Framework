using System.Net.Mail;

namespace TIKSN.Licensing;

public sealed record class OrganizationParty(
    string LongName,
    string ShortName,
    MailAddress Email,
    Uri Website) : Party(
        Email,
        Website);
