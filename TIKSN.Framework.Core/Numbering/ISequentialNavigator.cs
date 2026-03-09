using LanguageExt;

namespace TIKSN.Numbering;

public interface ISequentialNavigator<TSelf>
{
    public Option<TSelf> GetNext();

    public Option<TSelf> GetPrevious();
}
