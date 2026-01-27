using LanguageExt;

namespace TIKSN.Numbering;

public interface ISerialNumber<TSelf> : ISerial<TSelf>
    where TSelf : ISerialNumber<TSelf>
{
    public Option<TSelf> GetPrevious();

    public Option<TSelf> GetNext();
}
