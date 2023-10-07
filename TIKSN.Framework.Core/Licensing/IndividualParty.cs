using System.Net.Mail;

namespace TIKSN.Licensing;

public sealed class IndividualParty : Party
{
    public IndividualParty(
        string firstName,
        string lastName,
        string fullName,
        MailAddress email,
        Uri website) : base(
            email,
            website)
    {
        this.FirstName = firstName;
        this.LastName = lastName;
        this.FullName = fullName;
    }

    public string FirstName { get; }
    public string FullName { get; }
    public string LastName { get; }
}
