namespace TIKSN.Identity;

public class ClaimNotFoundException : Exception
{
    public ClaimNotFoundException() : base("Claim not found.")
    {
    }

    public ClaimNotFoundException(string message) : base(message)
    {
    }

    public ClaimNotFoundException(string message, Exception inner) : base(message, inner)
    {
    }

    public ClaimNotFoundException(string message, string claimType) : base(message) => this.ClaimType = claimType;

    public ClaimNotFoundException(string message, string claimType, Exception inner) : base(message, inner) =>
        this.ClaimType = claimType;

    public string? ClaimType { get; }
}
