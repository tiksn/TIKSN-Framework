using TIKSN.Data;
using Web_Service.Data.Entities;

namespace Web_Service.Data.Repositories
{
	public class RegionRepository : EntityRepositoryBase<RegionEntity>
	{
		public RegionRepository(InternationalizationContext context) : base(context)
		{

		}
    }
}
