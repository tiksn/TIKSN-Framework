using TIKSN.Data;
using Web_Service.Data.Entities;

namespace Web_Service.Data.Repositories
{
	public class CurrencyRepository: EntityRepositoryBase<CurrencyEntity>
	{
		public CurrencyRepository(InternationalizationContext context) : base(context)
		{

		}
    }
}
