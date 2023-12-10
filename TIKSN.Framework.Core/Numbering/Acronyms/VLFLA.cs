namespace TIKSN.Numbering.Acronyms;

public sealed class VLFLA : Acronym<VLFLA>, IAcronymLength<VLFLA>
{
    private VLFLA(string value) : base(value)
    {
    }

    public static int LetterCount => 5;
}
