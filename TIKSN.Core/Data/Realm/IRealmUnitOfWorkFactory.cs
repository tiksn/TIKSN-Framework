using Realms.Sync;
using System.Threading.Tasks;

namespace TIKSN.Data.Realm
{
    public interface IRealmUnitOfWorkFactory
    {
        Task LoginAsync(Credentials credentials);

        Task LogoutAsync();

        Task<IRealmUnitOfWork> CreateAsync();
    }
}
