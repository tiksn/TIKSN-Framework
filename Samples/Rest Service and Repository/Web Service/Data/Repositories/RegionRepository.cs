using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TIKSN.Data;
using Web_Service.Data.Entities;

namespace Web_Service.Data.Repositories
{
    public class RegionRepository : EntityRepository<InternationalizationContext, RegionEntity>, IRegionRepository
	{
		public RegionRepository(InternationalizationContext context) : base(context)
		{

		}

        public async Task<IEnumerable<RegionEntity>> GetAllAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            return await Entities.ToListAsync(cancellationToken);
        }

        public async Task<RegionEntity> GetAsync(int ID, CancellationToken cancellationToken = default(CancellationToken))
        {
            return await Entities.SingleOrDefaultAsync(item => item.Id == ID);
        }
    }
}
