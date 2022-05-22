using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace TIKSN.Data.EntityFrameworkCore
{
    public abstract class EntityUnitOfWorkFactoryBase : IUnitOfWorkFactory
    {
        private readonly IServiceProvider serviceProvider;

        protected EntityUnitOfWorkFactoryBase(IServiceProvider serviceProvider) =>
            this.serviceProvider = serviceProvider;

        public IUnitOfWork Create()
        {
            var dbContexts = this.GetContexts();

            return new EntityUnitOfWork(dbContexts);
        }

        protected TContext GetContext<TContext>() where TContext : DbContext =>
            this.serviceProvider.GetRequiredService<TContext>();

        protected abstract DbContext[] GetContexts();
    }
}
