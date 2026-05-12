namespace TIKSN.Numbering.Acronyms;

public interface IAcronymLength
{
    public static abstract int MinimumLetterCount { get; }

    public static abstract int MaximumLetterCount { get; }
}
