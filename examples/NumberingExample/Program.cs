using Microsoft.Extensions.DependencyInjection;
using Spectre.Console;
using TIKSN.DependencyInjection;
using TIKSN.Numbering;

var services = new ServiceCollection();
services.AddFrameworkCore();

await using var serviceProvider = services.BuildServiceProvider();

AnsiConsole.MarkupLine("[bold cyan]TIKSN Numbering Example[/]");
AnsiConsole.MarkupLine("[grey]-----------------------------------------[/]");

AnsiConsole.MarkupLine("[yellow]Simple Serial Number[/]");
var simpleNumber1 = new SimpleSerialNumber<BB26, uint>(BB26.Parse("A", null), 1U);
var simpleNumber2 = new SimpleSerialNumber<BB26, uint>(BB26.Parse("A", null), 2U);
AnsiConsole.MarkupLine($"Number 1: {simpleNumber1}");
AnsiConsole.MarkupLine($"Number 2: {simpleNumber2}");
AnsiConsole.MarkupLine($"Equal? {simpleNumber1.Equals(simpleNumber2)}");

AnsiConsole.MarkupLine("\n[fuchsia]Number First Serial Number[/]");
var numFirst1 = new NumberFirstSerialNumber<uint, BB26>(1U, BB26.Parse("ABC", null));
var numFirst2 = new NumberFirstSerialNumber<uint, BB26>(2U, BB26.Parse("XYZ", null));
AnsiConsole.MarkupLine($"Serial 1: {numFirst1}");
AnsiConsole.MarkupLine($"Serial 2: {numFirst2}");

AnsiConsole.MarkupLine("\n[green]Variant Serial Number[/]");
var variant1 = new VariantSerialNumber<BB26, uint, BB26>(BB26.Parse("V", null), 1U, BB26.Parse("A", null));
var variant2 = new VariantSerialNumber<BB26, uint, BB26>(BB26.Parse("V", null), 1U, BB26.Parse("B", null));
AnsiConsole.MarkupLine($"Variant 1: {variant1}");
AnsiConsole.MarkupLine($"Variant 2: {variant2}");
AnsiConsole.MarkupLine($"Same Base Number? {variant1.Number.Equals(variant2.Number)}");

AnsiConsole.MarkupLine("\n[blue]A1 Notation[/]");
var a1Notation = new A1Notation<uint>(BB26.Parse("A", null), 1U);
var b2Notation = new A1Notation<uint>(BB26.Parse("B", null), 2U);
AnsiConsole.MarkupLine($"Cell 1: {a1Notation}");
AnsiConsole.MarkupLine($"Cell 2: {b2Notation}");

