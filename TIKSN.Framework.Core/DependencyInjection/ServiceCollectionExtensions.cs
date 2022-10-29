using System;
using System.Collections.Generic;
using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using TIKSN.Data;
using TIKSN.FileSystem;

namespace TIKSN.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddFrameworkPlatform(
            this IServiceCollection services,
            IReadOnlyList<MapperProfile> mapperProfiles = null,
            Action<IMapperConfigurationExpression> autoMapperConfigurationAction = null)
        {
            _ = services.AddFrameworkCore();

            services.TryAddSingleton<IKnownFolders, KnownFolders>();

            mapperProfiles ??= Array.Empty<MapperProfile>();

            _ = services.AddAutoMapper(config =>
            {
                foreach (var mapperProfile in mapperProfiles)
                {
                    config.AddProfile(mapperProfile);
                }

                autoMapperConfigurationAction?.Invoke(config);
            });

            return services;
        }
    }
}
