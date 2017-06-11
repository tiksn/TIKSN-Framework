using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TIKSN.Data;
using Web_Service.Data.Entities;

namespace Web_Service.Data.Repositories
{
	public class CurrencyRepository: EntityRepository<InternationalizationContext, CurrencyEntity>, ICurrencyRepository
	{
		public CurrencyRepository(InternationalizationContext context) : base(context)
		{

		}

		public async Task<IEnumerable<CurrencyEntity>> GetAllAsync(CancellationToken cancellationToken)
		{
			return await Entities.ToListAsync(cancellationToken);
		}

		public async Task<CurrencyEntity> GetAsync(int ID, CancellationToken cancellationToken = default(CancellationToken))
		{
			return await Entities.SingleOrDefaultAsync(item => item.Id == ID);
		}
	}
}
