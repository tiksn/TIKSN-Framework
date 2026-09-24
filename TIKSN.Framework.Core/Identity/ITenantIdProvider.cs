using LanguageExt;

namespace TIKSN.Identity;

public interface ITenantIdProvider<T>
{
    public Option<T> FindTenantId();

    public T GetTenantId();
}
