using Microsoft.Extensions.DependencyInjection;
using System;

namespace TIKSN.Framework.UnitTests.DI
{
    public static class Dependencies
    {
        private static Lazy<IServiceProvider> lazyServiceProvider = new Lazy<IServiceProvider>(CreateServiceProvider);

        public static IServiceCollection ServiceCollection { get; } = new ServiceCollection();

        public static IServiceProvider ServiceProvider
        {
            get
            {
                return lazyServiceProvider.Value;
            }
        }

        private static IServiceProvider CreateServiceProvider()
        {
            return ServiceCollection.BuildServiceProvider();
        }
    }
}