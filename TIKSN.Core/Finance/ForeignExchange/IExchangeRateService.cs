using System.Threading;
using System.Threading.Tasks;

namespace TIKSN.Finance.ForeignExchange
{
    public interface IExchangeRateService
    {
        Task InitializeAsync(CancellationToken cancellationToken = default);
    }
}