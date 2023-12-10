namespace TIKSN.Numbering.Acronyms;

public sealed class LFLA : Acronym<LFLA>, IAcronymLength<LFLA>
{
    private LFLA(string value) : base(value)
    {
    }

    public static int LetterCount => 4;
}
