using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace TIKSN.Data.EntityFrameworkCore;

public abstract class EntityUnitOfWorkFactoryBase : IUnitOfWorkFactory
{
    private readonly IServiceProvider serviceProvider;

    protected EntityUnitOfWorkFactoryBase(IServiceProvider serviceProvider) =>
        this.serviceProvider = serviceProvider;

    public Task<IUnitOfWork> CreateAsync(CancellationToken cancellationToken)
    {
        var dbContexts = this.GetContexts();

        return Task.FromResult<IUnitOfWork>(new EntityUnitOfWork(dbContexts, this.serviceProvider));
    }

    protected TContext GetContext<TContext>() where TContext : DbContext =>
        this.serviceProvider.GetRequiredService<TContext>();

    protected abstract DbContext[] GetContexts();
}
