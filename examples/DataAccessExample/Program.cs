using LiteDB;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Spectre.Console;
using TIKSN.Data;
using TIKSN.Data.LiteDB;
using TIKSN.DependencyInjection;

Directory.CreateDirectory(".trash");

var configurationBuilder = new ConfigurationBuilder();
configurationBuilder.AddInMemoryCollection(new Dictionary<string, string?>
{
    { "ConnectionStrings:LiteDB", "Filename=.trash/examples.db;Mode=Exclusive" }
});
var configuration = configurationBuilder.Build();

var services = new ServiceCollection();
services.AddSingleton<IConfiguration>(configuration);
services.AddFrameworkCore();

// Register LiteDB database provider
services.AddSingleton<ILiteDbDatabaseProvider>(sp =>
    new LiteDbDatabaseProvider(sp.GetRequiredService<IConfiguration>(), "LiteDB"));

// Register the repository
services.AddSingleton<IProductRepository, ProductRepository>();

await using var serviceProvider = services.BuildServiceProvider();

var repository = serviceProvider.GetRequiredService<IProductRepository>();

AnsiConsole.MarkupLine("[bold cyan]TIKSN Data Access Example (LiteDB)[/]");
AnsiConsole.MarkupLine("[grey]-----------------------------------------[/]");

var productId = Guid.NewGuid();
var product = new ProductEntity(productId, "TIKSN Framework License", 99.99m);

// 1. Add
AnsiConsole.MarkupLine($"[yellow]Adding Product:[/] {product.Name}");
await repository.AddAsync(product, CancellationToken.None).ConfigureAwait(false);

// 2. Get
var retrievedProduct = await repository.GetAsync(productId, CancellationToken.None).ConfigureAwait(false);
AnsiConsole.MarkupLine($"[yellow]Retrieved Product:[/] {retrievedProduct.Name} - {retrievedProduct.Price:C}");

// 3. Update
retrievedProduct.Price = 79.99m;
AnsiConsole.MarkupLine($"[yellow]Updating Product Price:[/] {retrievedProduct.Price:C}");
await repository.UpdateAsync(retrievedProduct, CancellationToken.None).ConfigureAwait(false);

var updatedProduct = await repository.GetAsync(productId, CancellationToken.None).ConfigureAwait(false);
AnsiConsole.MarkupLine($"[yellow]Verified Updated Price:[/] {updatedProduct.Price:C}");

// 4. Delete
AnsiConsole.MarkupLine($"[yellow]Removing Product:[/] {productId}");
await repository.RemoveAsync(updatedProduct, CancellationToken.None).ConfigureAwait(false);

var exists = await repository.ExistsAsync(productId, CancellationToken.None).ConfigureAwait(false);
AnsiConsole.MarkupLine($"[yellow]Product Exists After Deletion?[/] [bold {(exists ? "red" : "green")}]{exists}[/]");

#pragma warning disable CA1050 // Declare types in namespaces
#pragma warning disable S3903 // Types should be defined in named namespaces
#pragma warning disable RCS1110 // Declare type inside namespace
public class ProductEntity : IEntity<Guid>
{
    public ProductEntity(Guid id, string name, decimal price)
    {
        ID = id;
        Name = name;
        Price = price;
    }

    [BsonId]
    public Guid ID { get; set; }
    public string Name { get; set; }
    public decimal Price { get; set; }
}

#pragma warning disable RCS1251 // Remove unnecessary braces
public interface IProductRepository : IRepository<ProductEntity>, IQueryRepository<ProductEntity, Guid>
{
}
#pragma warning restore RCS1251 // Remove unnecessary braces

public class ProductRepository : LiteDbRepository<ProductEntity, Guid>, IProductRepository
{
    public ProductRepository(ILiteDbDatabaseProvider databaseProvider)
        : base(databaseProvider, "Products", id => new BsonValue(id))
    {
    }
}
#pragma warning restore RCS1110 // Declare type inside namespace
#pragma warning restore S3903 // Types should be defined in named namespaces
#pragma warning restore CA1050 // Declare types in namespaces
