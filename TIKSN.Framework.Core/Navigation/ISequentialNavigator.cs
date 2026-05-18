using LanguageExt;

namespace TIKSN.Navigation;

public interface ISequentialNavigator<TSelf>
{
    public Option<TSelf> GetNext();

    public Option<TSelf> GetPrevious();
}
