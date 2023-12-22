namespace TIKSN.Numbering.Acronyms;

public sealed class LFLA : Acronym<LFLA>, IAcronymLength
{
    protected LFLA(string value) : base(value)
    {
    }

    public static int LetterCount => 4;
}
