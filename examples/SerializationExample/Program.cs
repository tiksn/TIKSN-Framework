using Microsoft.Extensions.DependencyInjection;
using Spectre.Console;
using TIKSN.DependencyInjection;
using TIKSN.Serialization;

var services = new ServiceCollection();
services.AddFrameworkCore();

// Register Serializers
services.AddSingleton<ISerializer<string>, JsonSerializer>();
services.AddSingleton<IDeserializer<string>, JsonDeserializer>();
services.AddSingleton<ISerializer<string>, DotNetXmlSerializer>();
services.AddSingleton<IDeserializer<string>, DotNetXmlDeserializer>();

await using var serviceProvider = services.BuildServiceProvider();

AnsiConsole.MarkupLine("[bold cyan]TIKSN Serialization Example[/]");
AnsiConsole.MarkupLine("[grey]-----------------------------------------[/]");

var sampleData = new SampleData
{
    Id = 12345,
    Name = "TIKSN Framework",
    Tags = new List<string> { "Serialization", "Example", "JSON", "XML" }
};

// JSON Serialization
var jsonSerializer = serviceProvider.GetServices<ISerializer<string>>().OfType<JsonSerializer>().Single();
var jsonDeserializer = serviceProvider.GetServices<IDeserializer<string>>().OfType<JsonDeserializer>().Single();

var jsonString = jsonSerializer.Serialize(sampleData);
AnsiConsole.MarkupLine("[yellow]JSON Serialized:[/]");
AnsiConsole.WriteLine(jsonString);

var jsonDeserialized = jsonDeserializer.Deserialize<SampleData>(jsonString);
AnsiConsole.MarkupLine($"[green]JSON Deserialized Name:[/] {jsonDeserialized?.Name}");

// XML Serialization
var xmlSerializer = serviceProvider.GetServices<ISerializer<string>>().OfType<DotNetXmlSerializer>().Single();
var xmlDeserializer = serviceProvider.GetServices<IDeserializer<string>>().OfType<DotNetXmlDeserializer>().Single();

var xmlString = xmlSerializer.Serialize(sampleData);
AnsiConsole.MarkupLine("\n[fuchsia]XML Serialized:[/]");
AnsiConsole.WriteLine(xmlString);

var xmlDeserialized = xmlDeserializer.Deserialize<SampleData>(xmlString);
AnsiConsole.MarkupLine($"[green]XML Deserialized Name:[/] {xmlDeserialized?.Name}");

#pragma warning disable CA1050 // Declare types in namespaces
#pragma warning disable S3903 // Types should be defined in named namespaces
#pragma warning disable RCS1110 // Declare type inside namespace
public class SampleData
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public List<string> Tags { get; set; } = new List<string>();
}
#pragma warning restore RCS1110 // Declare type inside namespace
#pragma warning restore S3903 // Types should be defined in named namespaces
#pragma warning restore CA1050 // Declare types in namespaces
