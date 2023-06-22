namespace TIKSN.Numbering.Acronyms;

public sealed class TLA : Acronym<TLA>
{
    private static readonly int LetterCount = 3;

    private TLA(string value) : base(value)
    {
    }
}
