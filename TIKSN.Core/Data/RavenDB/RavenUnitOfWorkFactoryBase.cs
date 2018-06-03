using Microsoft.Extensions.Options;
using Raven.Client.Documents;
using System;

namespace TIKSN.Data.RavenDB
{
    public abstract class RavenUnitOfWorkFactoryBase<TUnitOfWork> : IRavenUnitOfWorkFactory<TUnitOfWork>, IDisposable
        where TUnitOfWork : IUnitOfWork
    {
        private readonly IDocumentStore _store;

        protected RavenUnitOfWorkFactoryBase(IOptions<RavenUnitOfWorkFactoryOptions<TUnitOfWork>> options)
        {
            if (options == null)
                throw new ArgumentNullException(nameof(options));

            _store = new DocumentStore { Urls = options.Value.Urls, Database = options.Value.Database }.Initialize();
        }

        public TUnitOfWork Create()
        {
            return Create(_store);
        }

        public void Dispose()
        {
            _store.Dispose();
        }

        protected abstract TUnitOfWork Create(IDocumentStore store);
    }
}