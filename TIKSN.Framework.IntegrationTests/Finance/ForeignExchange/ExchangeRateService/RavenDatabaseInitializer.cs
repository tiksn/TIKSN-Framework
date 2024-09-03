using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Raven.Client.Documents;
using Raven.Client.Documents.Operations;
using Raven.Client.Exceptions;
using Raven.Client.Exceptions.Database;
using Raven.Client.ServerWide;
using Raven.Client.ServerWide.Operations;
using TIKSN.Data.RavenDB;

namespace TIKSN.IntegrationTests.Finance.ForeignExchange.ExchangeRateService;

public class RavenDatabaseInitializer : IDatabaseInitializer
{
    private readonly IOptions<RavenUnitOfWorkFactoryOptions> options;

    public RavenDatabaseInitializer(
        IOptions<RavenUnitOfWorkFactoryOptions> options)
        => this.options = options ?? throw new ArgumentNullException(nameof(options));

    public async Task InitializeAsync(CancellationToken cancellationToken)
    {
        var store =
            new DocumentStore { Urls = [.. this.options.Value.Urls], Database = this.options.Value.Database }
            .Initialize();

        _ = store.Operations.ForDatabase(this.options.Value.Database);
        await EnsureDatabaseExistsAsync(store);
    }

    private static async Task EnsureDatabaseExistsAsync(IDocumentStore store)
    {
        if (string.IsNullOrWhiteSpace(store.Database))
        {
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(store));
        }

        try
        {
            _ = await store.Maintenance.ForDatabase(store.Database).SendAsync(new GetStatisticsOperation());
        }
        catch (DatabaseDoesNotExistException)
        {
            try
            {
                _ = await store.Maintenance.Server.SendAsync(new CreateDatabaseOperation(new DatabaseRecord(store.Database)));
            }
            catch (ConcurrencyException)
            {
                throw;
            }
        }
    }
}
