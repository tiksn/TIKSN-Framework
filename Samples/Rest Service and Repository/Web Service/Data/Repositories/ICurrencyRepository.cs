using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TIKSN.Data;
using Web_Service.Data.Entities;

namespace Web_Service.Data.Repositories
{
	public interface ICurrencyRepository : IRepository<CurrencyEntity>
	{
		Task<CurrencyEntity> GetAsync(int ID, CancellationToken cancellationToken = default(CancellationToken));

		Task<IEnumerable<CurrencyEntity>> GetAllAsync(CancellationToken cancellationToken = default(CancellationToken));
	}
}
