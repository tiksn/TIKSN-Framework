namespace TIKSN.Numbering.Acronyms;

public sealed class TLA : Acronym<TLA>, IAcronymLength<TLA>
{
    private TLA(string value) : base(value)
    {
    }

    public static int LetterCount => 3;
}
