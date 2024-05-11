using LanguageExt;

namespace TIKSN.Numbering;

public interface ISerial<TSelf> : ISpanParsable<TSelf>, ISpanFormattable, IEquatable<TSelf>
    where TSelf : ISerial<TSelf>
{
#pragma warning disable CA1000 // Do not declare static members on generic types

    static abstract Option<TSelf> Parse(string s, bool asciiOnly, IFormatProvider? provider);

    static abstract Option<TSelf> Parse(ReadOnlySpan<char> s, bool asciiOnly, IFormatProvider? provider);

#pragma warning restore CA1000 // Do not declare static members on generic types
}
