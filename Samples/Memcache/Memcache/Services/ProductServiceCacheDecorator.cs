using System;
using System.Threading.Tasks;
using Memcache.Models;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using TIKSN.Data.Cache;

namespace Memcache.Services
{
	public class ProductServiceCacheDecorator : MemoryCacheDecoratorBase<ProductModel>, IProductService
    {
        private readonly IProductService _originalService;

        public ProductServiceCacheDecorator(IProductService originalService, IMemoryCache memoryCache, IOptions<MemoryCacheDecoratorOptions> genericOptions, IOptions<MemoryCacheDecoratorOptions<ProductModel>> specificOptions) : base(memoryCache, genericOptions, specificOptions)
        {
            _originalService = originalService;
        }

        public ProductModel GetProduct(int number)
        {
            var cacheKey = Tuple.Create(CacheKind.Product, number);

            return GetFromMemoryCache(cacheKey, () => _originalService.GetProduct(number));
        }

        public Task<ProductModel> GetProductAsync(int number)
        {
            var cacheKey = Tuple.Create(CacheKind.Product, number);

            return GetFromMemoryCacheAsync(cacheKey, () => _originalService.GetProductAsync(number));
        }
    }
}
