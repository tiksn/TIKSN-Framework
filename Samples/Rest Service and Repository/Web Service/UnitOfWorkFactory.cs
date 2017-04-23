using TIKSN.Data;
using Web_Service.Data.Entities;

namespace Web_Service
{
	public class UnitOfWorkFactory : IUnitOfWorkFactory
	{
		private readonly InternationalizationContext context;

		public UnitOfWorkFactory(InternationalizationContext context)
		{
			this.context = context;
		}

		public IUnitOfWork Create()
		{
			return new EntityUnitOfWork(context);
		}
	}
}
