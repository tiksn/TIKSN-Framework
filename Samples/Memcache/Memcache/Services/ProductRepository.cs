using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Memcache.Models;

namespace Memcache.Services
{
    public class ProductRepository : IProductRepository
    {
        public async Task AddAsync(ProductModel entity, CancellationToken cancellationToken)
        {
        }

        public async Task AddRangeAsync(IEnumerable<ProductModel> entities, CancellationToken cancellationToken = default(CancellationToken))
        {
        }

        public async Task<ProductModel> GetByIdAsync(Guid id)
        {
            return new ProductModel
            {
                ID = id,
                Name = $"Product {-1}"
            };
        }

        public async Task<ProductModel> GetByNameAsync(string name)
        {
            return new ProductModel
            {
                ID = Guid.NewGuid(),
                Name = $"Product {-2}"
            };
        }

        public async Task<ProductModel> GetByPriceRangeAsync(decimal min, decimal max)
        {
            return new ProductModel
            {
                ID = Guid.NewGuid(),
                Name = $"Product {-3}"
            };
        }

        public async Task RemoveAsync(ProductModel entity, CancellationToken cancellationToken = default(CancellationToken))
        {
        }

        public async Task RemoveRangeAsync(IEnumerable<ProductModel> entities, CancellationToken cancellationToken = default(CancellationToken))
        {
        }

        public async Task UpdateAsync(ProductModel entity, CancellationToken cancellationToken = default(CancellationToken))
        {
        }

        public async Task UpdateRangeAsync(IEnumerable<ProductModel> entities, CancellationToken cancellationToken = default(CancellationToken))
        {
        }
    }
}
