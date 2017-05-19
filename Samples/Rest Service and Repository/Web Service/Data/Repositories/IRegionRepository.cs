using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TIKSN.Data;
using Web_Service.Data.Entities;

namespace Web_Service.Data.Repositories
{
    public interface IRegionRepository : IRepository<RegionEntity>
    {
        Task<RegionEntity> GetAsync(int ID, CancellationToken cancellationToken = default(CancellationToken));

        Task<IEnumerable<RegionEntity>> GetAllAsync(CancellationToken cancellationToken = default(CancellationToken));
    }
}
