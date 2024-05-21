using Microsoft.Extensions.DependencyInjection;

namespace TIKSN.Data;

public class UnitOfWorkFactory<TKey> : IUnitOfWorkFactory<TKey>
{
    private readonly IServiceProvider serviceProvider;

    public UnitOfWorkFactory(
        IServiceProvider serviceProvider)
        => this.serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));

    public Task<IUnitOfWork> CreateAsync(
        TKey key,
        CancellationToken cancellationToken)
    {
        var unitOfWorkFactory = this.serviceProvider.GetRequiredKeyedService<IUnitOfWorkFactory>(key);

        return unitOfWorkFactory.CreateAsync(cancellationToken);
    }
}
