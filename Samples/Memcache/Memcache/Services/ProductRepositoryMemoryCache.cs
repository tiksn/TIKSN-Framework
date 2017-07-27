using System;
using System.Threading.Tasks;
using Memcache.Models;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using TIKSN.Data.Cache;

namespace Memcache.Services
{
    public class ProductRepositoryMemoryCache : RepositoryMemoryCacheDecorator<ProductModel, Guid>, IProductRepository
    {
        private readonly IProductRepository _repository;

        public ProductRepositoryMemoryCache(IProductRepository repository, IMemoryCache memoryCache, IOptions<MemoryCacheDecoratorOptions> genericOptions, IOptions<MemoryCacheDecoratorOptions<ProductModel>> specificOptions)
            : base(repository, memoryCache, genericOptions, specificOptions)
        {
            _repository = repository;
        }

        public Task<ProductModel> GetByIdAsync(Guid id)
        {
            var cacheKey = Tuple.Create(entityType, MemoryCacheKeyKind.Entity, id);

            return GetQueryFromMemoryCacheAsync(cacheKey, () => _repository.GetByIdAsync(id));
        }

        public Task<ProductModel> GetByNameAsync(string name)
        {
            var cacheKey = Tuple.Create(entityType, MemoryCacheKeyKind.Query, name);

            return GetQueryFromMemoryCacheAsync(cacheKey, () => _repository.GetByNameAsync(name));
        }

        public Task<ProductModel> GetByPriceRangeAsync(decimal min, decimal max)
        {
            var cacheKey = Tuple.Create(entityType, MemoryCacheKeyKind.Query, min, max);

            return GetQueryFromMemoryCacheAsync(cacheKey, () => _repository.GetByPriceRangeAsync(min, max));
        }
    }
}
