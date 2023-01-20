using System;
using System.Collections.Generic;
using System.Linq;
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
            mapperProfiles = mapperProfiles.Concat(GetPlatformMappers(services)).ToArray();

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

        private static IEnumerable<MapperProfile> GetPlatformMappers(IServiceCollection services)
        {
            yield return new Finance.ForeignExchange.LiteDB.DataEntityMapperProfile(services);
            yield return new Finance.ForeignExchange.EntityFrameworkCore.DataEntityMapperProfile(services);
            yield return new Finance.ForeignExchange.Mongo.DataEntityMapperProfile(services);
        }
    }
}
