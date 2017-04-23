using TIKSN.Data;
using Web_Service.Data.Entities;

namespace Web_Service.Data.Repositories
{
	public class RegionRepository : EntityRepository<InternationalizationContext, RegionEntity>, IRegionRepository
	{
		public RegionRepository(InternationalizationContext context) : base(context)
		{

		}
    }
}
