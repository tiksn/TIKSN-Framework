using Microsoft.Extensions.Caching.Memory;
using System;
using Memcache.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Memcache
{
	class Program
    {
        static void Main(string[] args)
        {
            var compositionRoot = new CompositionRoot();
            var serviceProvider = compositionRoot.CreateServiceProvider();
            var memoryCache = serviceProvider.GetRequiredService<IMemoryCache>();
            var productService = serviceProvider.GetRequiredService<IProductService>();
            var productRepository = serviceProvider.GetRequiredService<IProductRepository>();

            while (true)
            {
                var product = productService.GetProduct(12);

                Console.WriteLine($"ID: {product.ID} Name: {product.Name}");

                product = productService.GetProductAsync(12).Result;

                Console.WriteLine($"ID: {product.ID} Name: {product.Name}");

                product = productRepository.GetByNameAsync("xxx").Result;

                Console.WriteLine($"ID: {product.ID} Name: {product.Name}");

                Console.ReadLine();
            }
        }
    }
}