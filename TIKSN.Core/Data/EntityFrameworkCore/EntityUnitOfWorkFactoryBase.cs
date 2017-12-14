using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace TIKSN.Data.EntityFrameworkCore
{
    public abstract class EntityUnitOfWorkFactoryBase : IUnitOfWorkFactory
    {
        private readonly IServiceProvider serviceProvider;

        protected EntityUnitOfWorkFactoryBase(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }

        public IUnitOfWork Create()
        {
            var dbContexts = GetContexts();

            return new EntityUnitOfWork(dbContexts);
        }

        protected abstract DbContext[] GetContexts();

        protected TContext GetContext<TContext>() where TContext : DbContext
        {
            return serviceProvider.GetRequiredService<TContext>();
        }
    }
}