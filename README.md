# TIKSN Framework

[![Version](https://img.shields.io/nuget/v/TIKSN-Framework.svg)](https://www.nuget.org/packages/TIKSN-Framework)
[![NuGet Pre Release](https://img.shields.io/nuget/vpre/TIKSN-Framework.svg)](https://www.nuget.org/packages/TIKSN-Framework)
[![Developed by TIKSN Lab](https://img.shields.io/badge/Developed%20by-TIKSN%20Lab-orange.svg)](https://tiksn.com/project/tiksn-framework/)
[![StandWithUkraine](https://raw.githubusercontent.com/vshymanskyy/StandWithUkraine/main/badges/StandWithUkraine.svg)](https://github.com/vshymanskyy/StandWithUkraine/blob/main/docs/README.md)

## Key features are

* Versioning
* Finance, Currency
* Foreign Exchange
* Money
* Pricing strategy
* Telemetry
* Licensing
* Composite Weighted Progress
* Repository and Unity of Wok pattern implementation with Entity Framework Core
* Network Connectivity Service and Triggering
* Settings
* Windows Registry configuration source
* Azure Storage Repository
* MongoDB Repository
* NoDB Repository
* Lingual and Regional Localization
* Serialization
* Rest Requester
* Rest Repository
* Dependency Injection
* Composition Root Setup base classes

## Setup for Web Application

```csharp
using Autofac;
using Autofac.Extensions.DependencyInjection;
using TIKSN.DependencyInjection;
using TIKSN.Mapping;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());

builder.Services.AddFrameworkPlatform();

// Optional: Bulk register mappers or it can be done one by one
builder.Services.Scan(scan => scan
    .FromApplicationDependencies()
        .AddClasses(classes => classes.AssignableTo(typeof(IMapper<,>)))
            .AsImplementedInterfaces());

builder.Host.ConfigureContainer<ContainerBuilder>(containerBuilder =>
{
  containerBuilder.RegisterModule<CoreModule>();
  // Optional: Register project modules
});

var app = builder.Build();

await app.RunAsync().ConfigureAwait(false);

```

## Acknowledgments

* [JetBrains](https://www.jetbrains.com/?from=TIKSN-Framework) for providing free license.
