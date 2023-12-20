using System.Net.Mail;

namespace TIKSN.Licensing;

public abstract record class Party(
    MailAddress Email,
    Uri Website);
