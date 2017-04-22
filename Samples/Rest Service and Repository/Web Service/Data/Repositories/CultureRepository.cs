using TIKSN.Data;
using Web_Service.Data.Entities;

namespace Web_Service.Data.Repositories
{
	public class CultureRepository : EntityRepositoryBase<CultureEntity>, ICultureRepository
	{
		public CultureRepository(InternationalizationContext context) : base(context)
		{

		}
	}
}
