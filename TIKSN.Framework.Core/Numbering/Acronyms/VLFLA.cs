namespace TIKSN.Numbering.Acronyms;

#pragma warning disable S101 // Types should be named in PascalCase
#pragma warning disable MA0095 // A class that implements IEquatable<T> should override Equals(object)

public sealed class VLFLA : Acronym<VLFLA>, IAcronymLength
#pragma warning restore MA0095 // A class that implements IEquatable<T> should override Equals(object)
#pragma warning restore S101 // Types should be named in PascalCase
{
#pragma warning disable IDE0051 // Remove unused private members

    private VLFLA(string value) : base(value)
#pragma warning restore IDE0051 // Remove unused private members
    {
    }

    public static int LetterCount => 5;
}
