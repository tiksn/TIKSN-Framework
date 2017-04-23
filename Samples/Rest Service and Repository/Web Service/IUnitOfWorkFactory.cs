using TIKSN.Data;

namespace Web_Service
{
	public interface IUnitOfWorkFactory
    {
		IUnitOfWork Create();
	}
}
