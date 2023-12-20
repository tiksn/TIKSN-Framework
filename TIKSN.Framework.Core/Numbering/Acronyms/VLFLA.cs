namespace TIKSN.Numbering.Acronyms;

public sealed class VLFLA : Acronym<VLFLA>, IAcronymLength<VLFLA>
{
    protected VLFLA(string value) : base(value)
    {
    }

    public static int LetterCount => 5;
}
