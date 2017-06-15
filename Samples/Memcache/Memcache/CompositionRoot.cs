using Autofac;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using Memcache.Models;
using Memcache.Services;
using TIKSN.Data.Cache;
using TIKSN.DependencyInjection;

namespace Memcache
{
    public class CompositionRoot : AutofacPlatformCompositionRootSetupBase
    {
        protected override void ConfigureContainerBuilder(ContainerBuilder builder)
        {
            builder.RegisterMemoryCacheDecorator<ProductService, IProductService, ProductModel>((c, inner, mc, go, so) => new ProductServiceCacheDecorator(inner, mc, go, so));
            builder.RegisterMemoryCacheDecorator<ProductRepository, IProductRepository, ProductModel>((c, inner, mc, go, so) => new ProductRepositoryMemoryCache(inner, mc, go, so));
        }

        protected override void ConfigureOptions(IServiceCollection services, IConfigurationRoot configuration)
        {
            services.Configure<MemoryCacheDecoratorOptions>(options =>
            {
                options.AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(2);
            });
        }

        protected override void ConfigureServices(IServiceCollection services)
        {
            services.AddMemoryCache();
        }
    }
}
