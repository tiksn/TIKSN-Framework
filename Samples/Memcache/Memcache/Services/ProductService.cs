using System;
using System.Threading.Tasks;
using Memcache.Models;

namespace Memcache.Services
{
	public class ProductService : IProductService
    {
        public ProductModel GetProduct(int number)
        {
            return new ProductModel
            {
                ID = Guid.NewGuid(),
                Name = $"Product {number}"
            };
        }

        public async Task<ProductModel> GetProductAsync(int number)
        {
            return new ProductModel
            {
                ID = Guid.NewGuid(),
                Name = $"Product {number}"
            };
        }
    }
}
