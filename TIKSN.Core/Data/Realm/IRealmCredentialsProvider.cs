using Realms.Sync;
using System.Threading;
using System.Threading.Tasks;

namespace TIKSN.Data.Realm
{
    public interface IRealmCredentialsProvider
    {
        Task<Credentials> GetCredentialsAsync(CancellationToken cancellationToken);
    }
}
