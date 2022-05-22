using System;
using Autofac;
using Autofac.Builder;
using Autofac.Features.LightweightAdapters;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using TIKSN.Data.Cache.Distributed;
using TIKSN.Data.Cache.Memory;
using TIKSN.Serialization;

namespace TIKSN.DependencyInjection
{
    public static class AutofacExtensions
    {
        public static IRegistrationBuilder<TService, LightweightAdapterActivatorData, DynamicRegistrationStyle>
            RegisterMemoryCacheDecorator<TImplementer, TService, TEntity>(this ContainerBuilder builder,
                Func<IComponentContext, TService, IMemoryCache, IOptions<MemoryCacheDecoratorOptions>,
                    IOptions<MemoryCacheDecoratorOptions<TEntity>>, TService> decorator, object fromKey)
            where TImplementer : TService =>
            builder.RegisterSingleLevelDecorator<TImplementer, TService>((c, inner) => decorator(c, inner,
                    c.Resolve<IMemoryCache>(),
                    c.Resolve<IOptions<MemoryCacheDecoratorOptions>>(),
                    c.Resolve<IOptions<MemoryCacheDecoratorOptions<TEntity>>>()),
                fromKey);

        public static IRegistrationBuilder<TService, LightweightAdapterActivatorData, DynamicRegistrationStyle>
            RegisterDistributedCacheDecorator<TImplementer, TService, TEntity>(this ContainerBuilder builder,
                Func<IComponentContext, TService, IDistributedCache, ISerializer<byte[]>, IDeserializer<byte[]>,
                    IOptions<DistributedCacheDecoratorOptions>,
                    IOptions<DistributedCacheDecoratorOptions<TEntity>>, TService> decorator, object fromKey)
            where TImplementer : TService =>
            builder.RegisterSingleLevelDecorator<TImplementer, TService>((c, inner) => decorator(c, inner,
                    c.Resolve<IDistributedCache>(),
                    c.Resolve<ISerializer<byte[]>>(),
                    c.Resolve<IDeserializer<byte[]>>(),
                    c.Resolve<IOptions<DistributedCacheDecoratorOptions>>(),
                    c.Resolve<IOptions<DistributedCacheDecoratorOptions<TEntity>>>()),
                fromKey);

        public static IRegistrationBuilder<TService, LightweightAdapterActivatorData, DynamicRegistrationStyle>
            RegisterSingleLevelDecorator<TImplementer, TService>(this ContainerBuilder builder,
                Func<IComponentContext, TService, TService> decorator, object fromKey)
            where TImplementer : TService =>
            builder.RegisterDecorator(decorator, fromKey);
    }
}
