using LanguageExt;

namespace TIKSN.Numbering;

public interface ISerialNumber<TSelf> : ISerial<TSelf>
    where TSelf : ISerialNumber<TSelf>
{
    Option<TSelf> GetPrevious();

    Option<TSelf> GetNext();
}
