using Base62;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Localization;
using ReactiveUI;
using System;
using System.Numerics;
using TIKSN.FileSystem;
using TIKSN.Globalization;
using TIKSN.Serialization;
using TIKSN.Serialization.Bond;
using TIKSN.Serialization.MessagePack;
using TIKSN.Serialization.Numerics;
using TIKSN.Shell;
using TIKSN.Time;
using TIKSN.Web.Rest;

namespace TIKSN.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddFrameworkCore(this IServiceCollection services)
        {
            services.AddLocalization();
            services.AddMemoryCache();
            services.AddOptions();

            services.TryAddSingleton<IConsoleService, ConsoleService>();
            services.TryAddSingleton<ICultureFactory, CultureFactory>();
            services.TryAddSingleton<ICurrencyFactory, CurrencyFactory>();
            services.TryAddSingleton<IRegionFactory, RegionFactory>();
            services.TryAddSingleton<IResourceNamesCache, ResourceNamesCache>();
            services.TryAddSingleton<IShellCommandEngine, ShellCommandEngine>();
            services.TryAddSingleton<ITimeProvider, TimeProvider>();
            services.TryAddSingleton<Random>();
            services.TryAddSingleton(MsgPack.Serialization.SerializationContext.Default);
            services.TryAddSingleton<IKnownFolders, KnownFolders>();

            services.TryAddSingleton<CompactBinaryBondDeserializer>();
            services.TryAddSingleton<CompactBinaryBondSerializer>();
            services.TryAddSingleton<DotNetXmlDeserializer>();
            services.TryAddSingleton<DotNetXmlSerializer>();
            services.TryAddSingleton<FastBinaryBondDeserializer>();
            services.TryAddSingleton<FastBinaryBondSerializer>();
            services.TryAddSingleton<ICustomDeserializer<byte[], BigInteger>, UnsignedBigIntegerBinaryDeserializer>();
            services.TryAddSingleton<ICustomSerializer<byte[], BigInteger>, UnsignedBigIntegerBinarySerializer>();
            services.TryAddSingleton<IRestRequester, RestRequester>();
            services.TryAddSingleton<JsonDeserializer>();
            services.TryAddSingleton<JsonSerializer>();
            services.TryAddSingleton<MessagePackDeserializer>();
            services.TryAddSingleton<MessagePackSerializer>();
            services.TryAddSingleton<SimpleBinaryBondDeserializer>();
            services.TryAddSingleton<SimpleBinaryBondSerializer>();
            services.TryAddSingleton<SimpleJsonBondDeserializer>();
            services.TryAddSingleton<SimpleJsonBondSerializer>();
            services.TryAddSingleton<SimpleXmlBondDeserializer>();
            services.TryAddSingleton<SimpleXmlBondSerializer>();
            services.TryAddSingleton<Base62Converter>();

            services.TryAddScoped<IShellCommandContext, ShellCommandContext>();

            services.AddSingleton(MessageBus.Current);

            return services;
        }
    }
}