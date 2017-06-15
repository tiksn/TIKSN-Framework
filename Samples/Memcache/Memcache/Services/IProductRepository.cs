using System;
using System.Threading.Tasks;
using Memcache.Models;
using TIKSN.Data;

namespace Memcache.Services
{
    public interface IProductRepository : IRepository<ProductModel>
    {
        Task<ProductModel> GetByIdAsync(Guid id);

        Task<ProductModel> GetByNameAsync(string name);

        Task<ProductModel> GetByPriceRangeAsync(decimal min, decimal max);
    }
}
