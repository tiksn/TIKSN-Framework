using LanguageExt;

namespace TIKSN.Numbering;

public interface ISerial<TSelf> : ISpanParsable<TSelf>, ISpanFormattable, IEquatable<TSelf>
    where TSelf : ISerial<TSelf>
{
    static abstract Option<TSelf> Parse(string s, bool asciiOnly, IFormatProvider provider);

    static abstract Option<TSelf> Parse(ReadOnlySpan<char> s, bool asciiOnly, IFormatProvider provider);
}
