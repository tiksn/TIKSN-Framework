using Realms.Sync;
using System.Threading.Tasks;

namespace TIKSN.Data.Realm
{
    public interface IRealmUnitOfWorkFactory<TRealmUnitOfWork>
        where TRealmUnitOfWork : IUnitOfWork
    {
        Task LoginAsync(Credentials credentials);

        Task LogoutAsync();

        Task<TRealmUnitOfWork> CreateAsync();
    }
}
