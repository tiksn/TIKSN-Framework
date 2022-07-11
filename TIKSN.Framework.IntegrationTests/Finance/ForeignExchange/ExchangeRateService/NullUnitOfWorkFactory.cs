using System;
using System.Threading;
using System.Threading.Tasks;
using TIKSN.Data;

namespace TIKSN.Finance.ForeignExchange.ExchangeRateService.IntegrationTests
{
    public class NullUnitOfWorkFactory : IUnitOfWorkFactory
    {
        private readonly IServiceProvider serviceProvider;

        public NullUnitOfWorkFactory(IServiceProvider serviceProvider)
            => this.serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));

        public async Task<IUnitOfWork> CreateAsync(CancellationToken cancellationToken) => new NullUnitOfWork(this.serviceProvider);
    }
}