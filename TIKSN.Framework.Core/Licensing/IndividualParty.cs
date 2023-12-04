using System.Net.Mail;

namespace TIKSN.Licensing;

public sealed record class IndividualParty(
    string FirstName,
    string LastName,
    string FullName,
    MailAddress Email,
    Uri Website) : Party(
        Email,
        Website);
