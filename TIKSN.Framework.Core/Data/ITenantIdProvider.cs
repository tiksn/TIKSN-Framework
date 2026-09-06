using LanguageExt;

namespace TIKSN.Data;

public interface ITenantIdProvider<T>
{
    public Option<T> FindTenantId();

    public T GetTenantId();
}
