using NodaTime;
using Spectre.Console;
using TIKSN.Time;

AnsiConsole.MarkupLine("[bold cyan]TIKSN Time Abstractions Example[/]");
AnsiConsole.MarkupLine("[grey]-------------------------------[/]");

// Create an Academic Year (starts Sept 1 by default)
var academicYear = new AcademicYear(2023);
AnsiConsole.MarkupLine($"[bold yellow]Academic Year:[/] [green]{academicYear}[/]");

// Let's check some dates against this academic year
var date1 = new LocalDate(2023, 10, 15);
var date2 = new LocalDate(2024, 5, 20);
var date3 = new LocalDate(2023, 8, 30); // Before Sept 1

string FormatBool(bool value) => value ? $"[bold green]{value}[/]" : $"[bold red]{value}[/]";

AnsiConsole.MarkupLine($"Does [fuchsia]{academicYear}[/] contain [cyan]{date1}[/]? {FormatBool(academicYear.Contains(date1))}");
AnsiConsole.MarkupLine($"Does [fuchsia]{academicYear}[/] contain [cyan]{date2}[/]? {FormatBool(academicYear.Contains(date2))}");
AnsiConsole.MarkupLine($"Does [fuchsia]{academicYear}[/] contain [cyan]{date3}[/]? {FormatBool(academicYear.Contains(date3))}");

AnsiConsole.WriteLine();

// Create a Fiscal Year
// We define it explicitly starting on July 1st
var fiscalYear = new FiscalYear(2024, new AnnualDate(7, 1));
AnsiConsole.MarkupLine($"[bold yellow]Fiscal Year:[/] [green]{fiscalYear}[/]");
var fiscalDate = new LocalDate(2024, 7, 5);
AnsiConsole.MarkupLine($"Does [fuchsia]{fiscalYear}[/] contain [cyan]{fiscalDate}[/]? {FormatBool(fiscalYear.Contains(fiscalDate))}");

AnsiConsole.WriteLine();

// Working with CalendarMonth
var currentMonth = new CalendarMonth(2024, 3); // March 2024
AnsiConsole.MarkupLine($"[bold yellow]Calendar Month:[/] [green]{currentMonth}[/]");

var nextMonthOpt = currentMonth.GetNext();
nextMonthOpt.Match(
    next => AnsiConsole.MarkupLine($"Next month is: [bold green]{next}[/]"),
    () => AnsiConsole.MarkupLine("[bold red]Could not determine next month[/]")
);
