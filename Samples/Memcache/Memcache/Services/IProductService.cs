using System.Threading.Tasks;
using Memcache.Models;

namespace Memcache.Services
{
	public interface IProductService
    {
        ProductModel GetProduct(int number);

        Task<ProductModel> GetProductAsync(int number);
    }
}
