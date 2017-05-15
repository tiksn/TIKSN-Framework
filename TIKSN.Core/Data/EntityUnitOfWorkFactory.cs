using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace TIKSN.Data
{
	public class EntityUnitOfWorkFactory<TContext> : IUnitOfWorkFactory where TContext : DbContext
	{
		private readonly IServiceProvider serviceProvider;

		public EntityUnitOfWorkFactory(IServiceProvider serviceProvider)
		{
			this.serviceProvider = serviceProvider;
		}

		public IUnitOfWork Create()
		{
			var dbContext = serviceProvider.GetRequiredService<TContext>();

			return new EntityUnitOfWork(dbContext);
		}
	}
}