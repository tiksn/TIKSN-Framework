using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TIKSN.Data;
using Web_Service.Data.Entities;

namespace Web_Service.Data.Repositories
{
    public interface ICultureRepository : IRepository<CultureEntity>
    {
        Task<CultureEntity> GetAsync(int ID, CancellationToken cancellationToken = default(CancellationToken));

        Task<CultureEntity> GetByNameAsync(string name, CancellationToken cancellationToken = default(CancellationToken));

        Task<IEnumerable<CultureEntity>> GetAllAsync(CancellationToken cancellationToken = default(CancellationToken));
    }
}
