using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Spectre.Console;
using TIKSN.Data;
using TIKSN.DependencyInjection;
using TIKSN.Serialization;
using TIKSN.Web.Rest;

var services = new ServiceCollection();
services.AddFrameworkCore();

services.AddLogging(builder =>
{
    builder.SetMinimumLevel(LogLevel.Warning);
    builder.AddConsole();
});

// Configure options for the RestRepository
services.Configure<RestRepositoryOptions<UserEntity>>(options =>
{
    options.EndpointKey = "JsonPlaceholder";
    options.ResourceTemplate = "users/{ID}";
    options.MediaType = "application/json";
    options.Authentication = RestAuthenticationType.None;
});

#pragma warning disable S1075 // URIs should not be hardcoded
services.AddHttpClient("JsonPlaceholder", client => client.BaseAddress = new Uri("https://jsonplaceholder.typicode.com/"));
#pragma warning restore S1075 // URIs should not be hardcoded

// Register dummy token provider (not used since Authentication = None)
services.AddSingleton<IRestAuthenticationTokenProvider, DummyRestTokenProvider>();

// Register serializers and RestFactory
services.AddSingleton<JsonSerializer>();
services.AddSingleton<JsonDeserializer>();
services.AddSingleton<DotNetXmlSerializer>();
services.AddSingleton<DotNetXmlDeserializer>();
services.AddSingleton<SerializationRestFactory>();
services.AddSingleton<ISerializerRestFactory>(sp => sp.GetRequiredService<SerializationRestFactory>());
services.AddSingleton<IDeserializerRestFactory>(sp => sp.GetRequiredService<SerializationRestFactory>());

services.AddSingleton<IUserRepository, UserRepository>();

await using var serviceProvider = services.BuildServiceProvider();

var repository = serviceProvider.GetRequiredService<IUserRepository>();

AnsiConsole.MarkupLine("[bold cyan]TIKSN REST API Integration Example[/]");
AnsiConsole.MarkupLine("[grey]----------------------------------------[/]");

AnsiConsole.MarkupLine("[yellow]Fetching user with ID 1 from JSONPlaceholder...[/]");

try
{
    var user = await repository.GetAsync(1, CancellationToken.None).ConfigureAwait(false);

    AnsiConsole.MarkupLine("[bold green]User Found![/]");
    AnsiConsole.MarkupLine($"[fuchsia]Name:[/] {user.Name}");
    AnsiConsole.MarkupLine($"[fuchsia]Username:[/] {user.Username}");
    AnsiConsole.MarkupLine($"[fuchsia]Email:[/] {user.Email}");
}
catch (Exception ex)
{
    AnsiConsole.MarkupLine($"[bold red]Error:[/] {ex.Message}");
}

#pragma warning disable CA1050 // Declare types in namespaces
#pragma warning disable S3903 // Types should be defined in named namespaces
#pragma warning disable RCS1110 // Declare type inside namespace
public class UserEntity : IEntity<int>
{
    public int ID { get; set; }
    public required string Name { get; set; }
    public required string Username { get; set; }
    public required string Email { get; set; }
}

#pragma warning disable RCS1251 // Remove unnecessary braces
public interface IUserRepository : IRestRepository<UserEntity, int>
{
}
#pragma warning restore RCS1251 // Remove unnecessary braces

public class UserRepository : RestRepository<UserEntity, int>, IUserRepository
{
    public UserRepository(
        IHttpClientFactory httpClientFactory,
        ISerializerRestFactory serializerRestFactory,
        IDeserializerRestFactory deserializerRestFactory,
        IRestAuthenticationTokenProvider restAuthenticationTokenProvider,
        IOptions<RestRepositoryOptions<UserEntity>> options,
        ILogger<UserRepository> logger)
        : base(httpClientFactory, serializerRestFactory, deserializerRestFactory, restAuthenticationTokenProvider, options, logger)
    {
    }
}

public class DummyRestTokenProvider : IRestAuthenticationTokenProvider
{
    public Task<string> GetAuthenticationTokenAsync(string apiKey) => Task.FromResult(string.Empty);
}
#pragma warning restore RCS1110 // Declare type inside namespace
#pragma warning restore S3903 // Types should be defined in named namespaces
#pragma warning restore CA1050 // Declare types in namespaces
