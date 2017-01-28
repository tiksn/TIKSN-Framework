using System;
using System.Threading.Tasks;

namespace TIKSN.Data
{
	public interface IUnitOfWork : IDisposable
	{
		Task CompleteAsync();
	}
}