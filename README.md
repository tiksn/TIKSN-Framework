# TIKSN Framework

[![Version](https://img.shields.io/nuget/v/TIKSN-Framework.svg)](https://www.nuget.org/packages/TIKSN-Framework)
[![TIKSN Framework NuGet Package Downloads](https://img.shields.io/nuget/dt/TIKSN-Framework)](https://www.nuget.org/packages/TIKSN-Framework)
[![Developed by Tigran (TIKSN) Torosyan](https://img.shields.io/badge/Developed%20by-Tigran%20%28TIKSN%29%20Torosyan-orange.svg)](https://tiksn.com/project/tiksn-framework/)
[![StandWithUkraine](https://raw.githubusercontent.com/vshymanskyy/StandWithUkraine/main/badges/StandWithUkraine.svg)](https://github.com/vshymanskyy/StandWithUkraine/blob/main/docs/README.md)

TIKSN Framework is a .NET 10 application framework and utility library for building modular services, command-line tools, data-driven applications, and .NET MAUI apps. The package combines the core framework assembly, platform-specific MAUI helpers, and language/region localization resources.

## Package

Install the NuGet package:

```powershell
dotnet add package TIKSN-Framework
```

The package includes:

- `TIKSN.Framework.Core` for shared framework services and abstractions.
- `TIKSN.Framework.Maui` for Android, iOS, Mac Catalyst, and Windows MAUI targets.
- `TIKSN.LanguageLocalization` and `TIKSN.RegionLocalization` satellite resources.

Supported package target frameworks:

- `net10.0`
- `net10.0-android21.0`
- `net10.0-ios14.2`
- `net10.0-maccatalyst14.0`
- `net10.0-windows10.0.19041.0`

The repository uses the .NET SDK pinned in `global.json` (`10.0.100`, rolling forward to the latest major SDK when available).

## Feature Areas

- Dependency injection: `AddFrameworkCore`, Autofac `CoreModule`, and MAUI `AddFrameworkPlatform` platform registrations.
- Data access: repository, query repository, file repository, stream repository, unit-of-work abstractions, pagination helpers, and adapters for Entity Framework Core, Azure Table Storage, Azure Blob Storage, MongoDB, LiteDB, and RavenDB.
- Data caching: memory, distributed, and hybrid cache decorators for repository and query repository implementations.
- Finance: `Money`, currency metadata, currency pairs, fixed/composite/cached converters, pricing models, pricing strategies, and foreign exchange providers.
- Foreign exchange sources: Bank of Canada, Bank of England, Bank of Russia, Central Bank of Armenia, European Central Bank, Federal Reserve System, National Bank of Poland, National Bank of Ukraine, Reserve Bank of Australia, and Swiss National Bank.
- Globalization and localization: culture, country, currency, region, time zone, language localization, region localization, and composite string localizers.
- Serialization: JSON, XML, MessagePack, custom serializer/deserializer abstractions, unsigned `BigInteger` binary serialization, and Protocol Buffers schema support used by licensing.
- Licensing: license descriptors, license generation, entitlement conversion hooks, and RSA, DSA, and Ed25519 certificate signature services.
- Shell and PowerShell: shell command engine, command attributes, console services, progress reporting, user confirmation, and PowerShell logging/progress helpers.
- Integration: command/event/query marker interfaces and correlation ID services backed by GUID, ULID, CUID, and Base62 implementations.
- Application services: telemetry abstractions, configuration validation, settings, known folders, network connectivity, antimalware scanning abstractions, identity generation, mapping, numbering, versioning, time period types, web REST helpers, and sitemap models.
- MAUI platform support: platform modules and service collection extensions, plus Windows registry configuration/settings and Windows antimalware scanner integration.

## Repository Layout

- `TIKSN.Framework.Core`: core framework library.
- `TIKSN.Framework.Maui`: MAUI platform library.
- `TIKSN.LanguageLocalization`: language localization resources.
- `TIKSN.RegionLocalization`: region localization resources.
- `TIKSN.Framework.Core.Tests`: unit tests.
- `TIKSN.Framework.IntegrationTests`: integration tests.
- `examples/ShellCommands`: shell command sample.
- `examples/AntimalwareScanning`: antimalware scanning sample.

## Setup for ASP.NET Core

```csharp
using Autofac;
using Autofac.Extensions.DependencyInjection;
using TIKSN.DependencyInjection;
using TIKSN.Mapping;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());

builder.Services.AddFrameworkCore();

builder.Services.Scan(scan => scan
    .FromApplicationDependencies()
    .AddClasses(classes => classes.AssignableTo(typeof(IMapper<,>)))
    .AsImplementedInterfaces());

builder.Host.ConfigureContainer<ContainerBuilder>(containerBuilder =>
{
    containerBuilder.RegisterModule<CoreModule>();
});

var app = builder.Build();

await app.RunAsync().ConfigureAwait(false);
```

## Setup for Generic Host

```csharp
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TIKSN.DependencyInjection;

var host = Host.CreateDefaultBuilder(args)
    .UseServiceProviderFactory(new AutofacServiceProviderFactory())
    .ConfigureServices((_, services) =>
    {
        services.AddFrameworkCore();
    })
    .ConfigureContainer<ContainerBuilder>(containerBuilder =>
    {
        containerBuilder.RegisterModule<CoreModule>();
    })
    .Build();

await host.RunAsync().ConfigureAwait(false);
```

## Setup for .NET MAUI

Use the platform-specific extension from `TIKSN.Framework.Maui` in your MAUI startup:

```csharp
using TIKSN.DependencyInjection;

builder.Services.AddFrameworkPlatform();
```

`AddFrameworkPlatform` calls `AddFrameworkCore` and then adds platform-specific services where available.

## Build and Test

The repository uses PowerShell 7.4+ wrapper scripts around Invoke-Build tasks.

```powershell
./restore.ps1
./build.ps1
./test.ps1
./pack.ps1 -Version 4.5.0-alpha.0
```

Useful direct commands:

```powershell
dotnet restore "TIKSN Framework.slnx"
dotnet test TIKSN.Framework.Core.Tests/TIKSN.Framework.Core.Tests.csproj
dotnet test TIKSN.Framework.IntegrationTests/TIKSN.Framework.IntegrationTests.csproj
```

Integration tests can require external services such as MongoDB or RavenDB, depending on the test module being exercised.
