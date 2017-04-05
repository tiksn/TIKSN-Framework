using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace TIKSN.Data
{
	public interface IRepository<T>
	{
		Task AddAsync(T entity, CancellationToken cancellationToken = default(CancellationToken));

		Task AddRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default(CancellationToken));

		Task RemoveAsync(T entity, CancellationToken cancellationToken = default(CancellationToken));

		Task RemoveRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default(CancellationToken));

		Task UpdateAsync(T entity, CancellationToken cancellationToken = default(CancellationToken));

		Task UpdateRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default(CancellationToken));
	}
}