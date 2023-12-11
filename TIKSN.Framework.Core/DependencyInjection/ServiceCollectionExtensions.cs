using System.Net.Http.Headers;
using System.Numerics;
using MessagePack;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Localization;
using NodaTime;
using ReactiveUI;
using TIKSN.FileSystem;
using TIKSN.Finance.ForeignExchange.Bank;
using TIKSN.Finance.ForeignExchange.Cumulative;
using TIKSN.Globalization;
using TIKSN.Identity;
using TIKSN.Serialization;
using TIKSN.Serialization.Bond;
using TIKSN.Serialization.MessagePack;
using TIKSN.Serialization.Numerics;
using TIKSN.Shell;
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
        services.TryAddSingleton(TimeProvider.System);
        services.TryAddSingleton<IClock>(SystemClock.Instance);
        services.TryAddSingleton<Random>();
        services.TryAddSingleton(MessagePackSerializerOptions.Standard);
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

        _ = services.AddHttpClient<IBankOfCanada, BankOfCanada>();
        _ = services.AddHttpClient<IBankOfEngland, BankOfEngland>();
        _ = services.AddHttpClient<IBankOfRussia, BankOfRussia>();
        _ = services.AddHttpClient<ICentralBankOfArmenia, CentralBankOfArmenia>()
            .ConfigureHttpClient(config => config.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("TIKSN-Framework", typeof(ServiceCollectionExtensions).Assembly.GetName().Version.ToString())));
        _ = services.AddHttpClient<IEuropeanCentralBank, EuropeanCentralBank>();
        _ = services.AddHttpClient<IFederalReserveSystem, FederalReserveSystem>();
        _ = services.AddHttpClient<INationalBankOfPoland, NationalBankOfPoland>();
        _ = services.AddHttpClient<INationalBankOfUkraine, NationalBankOfUkraine>();
        _ = services.AddHttpClient<IReserveBankOfAustralia, ReserveBankOfAustralia>();
        _ = services.AddHttpClient<ISwissNationalBank, SwissNationalBank>();
        _ = services.AddHttpClient<ICurrencylayerDotCom, CurrencylayerDotCom>();

        return services;
    }
}
