using Autofac;
using Autofac.Builder;
using Autofac.Features.LightweightAdapters;
using System;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using TIKSN.Data.Cache;

namespace TIKSN.DependencyInjection
{
	public static class AutofacExtensions
	{
		public static IRegistrationBuilder<TService, LightweightAdapterActivatorData, DynamicRegistrationStyle>
			RegisterMemoryCacheDecorator<TImplementer, TService, TEntity>(this ContainerBuilder builder,
				Func<IComponentContext, TService, IMemoryCache, IOptions<MemoryCacheDecoratorOptions>,
				IOptions<MemoryCacheDecoratorOptions<TEntity>>, TService> decorator)
			where TImplementer : TService
		{
			builder.RegisterType<TImplementer>().Named<TService>("service");
			return builder.RegisterSingleLevelDecorator<TImplementer, TService>((c, inner) => decorator(c, inner, c.Resolve<IMemoryCache>(),
				c.Resolve<IOptions<MemoryCacheDecoratorOptions>>(),
				c.Resolve<IOptions<MemoryCacheDecoratorOptions<TEntity>>>()));
		}

		public static IRegistrationBuilder<TService, LightweightAdapterActivatorData, DynamicRegistrationStyle>
			RegisterSingleLevelDecorator<TImplementer, TService>(this ContainerBuilder builder, Func<IComponentContext, TService, TService> decorator)
			where TImplementer : TService
		{
			builder.RegisterType<TImplementer>().Named<TService>("service");
			return builder.RegisterDecorator(decorator, "service");
		}
	}
}
