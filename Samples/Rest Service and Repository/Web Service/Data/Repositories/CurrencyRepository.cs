using TIKSN.Data;
using Web_Service.Data.Entities;

namespace Web_Service.Data.Repositories
{
	public class CurrencyRepository: EntityRepository<InternationalizationContext, CurrencyEntity>, ICurrencyRepository
	{
		public CurrencyRepository(InternationalizationContext context) : base(context)
		{

		}
    }
}
