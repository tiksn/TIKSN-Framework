using System;
using System.Threading.Tasks;

namespace TIKSN.Data
{
	public abstract class UnitOfWorkBase : IUnitOfWork
	{
		public abstract Task CompleteAsync();

		public void Dispose()
		{
			if (IsDirty())
				throw new InvalidOperationException("Unit of work disposed without completion.");
		}

		protected abstract bool IsDirty();
	}
}