using System.Numerics;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Localization;
using MsgPack.Serialization;
using ReactiveUI;
using TIKSN.FileSystem;
using TIKSN.Globalization;
using TIKSN.Identity;
using TIKSN.Serialization;
using TIKSN.Serialization.Bond;
using TIKSN.Serialization.MessagePack;
using TIKSN.Serialization.Numerics;
using TIKSN.Shell;
using TIKSN.Time;
using TIKSN.Web.Rest;
using MessagePackSerializer = TIKSN.Serialization.MessagePack.MessagePackSerializer;

namespace TIKSN.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddFrameworkCore(this IServiceCollection services)
    {
        _ = services.AddLocalization();
        _ = services.AddMemoryCache();
        _ = services.AddHttpClient();
        _ = services.AddOptions();

        services.TryAddSingleton<ICultureFactory, CultureFactory>();
        services.TryAddSingleton<ICurrencyFactory, CurrencyFactory>();
        services.TryAddSingleton<IRegionFactory, RegionFactory>();
        services.TryAddSingleton<IResourceNamesCache, ResourceNamesCache>();
        services.TryAddSingleton<IShellCommandEngine, ShellCommandEngine>();
        services.TryAddSingleton<ITimeProvider, TimeProvider>();
        services.TryAddSingleton<Random>();
        services.TryAddSingleton(SerializationContext.Default);
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
        services.TryAddSingleton<IIdentityGenerator<long>, IdGenIdentityGenerator>();

        services.TryAddScoped<IShellCommandContext, ShellCommandContext>();

        _ = services.AddSingleton(MessageBus.Current);

        services.TryAddSingleton<IConsoleService, ConsoleService>();
        services.TryAddSingleton<IKnownFolders, KnownFolders>();

        return services;
    }
}
