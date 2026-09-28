using LanguageExt;

namespace TIKSN.Identity;

public interface IUserIdProvider<T>
{
    public Option<T> FindUserId();

    public T GetUserId();
}
