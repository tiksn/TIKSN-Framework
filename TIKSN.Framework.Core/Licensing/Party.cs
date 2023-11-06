using System.Net.Mail;

namespace TIKSN.Licensing;

public abstract class Party
{
    protected Party(
        MailAddress email,
        Uri website)
    {
        this.Email = email;
        this.Website = website;
    }

    public MailAddress Email { get; }
    public Uri Website { get; }
}
