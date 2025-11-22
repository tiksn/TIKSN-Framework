# TIKSN Framework

[![Version](https://img.shields.io/nuget/v/TIKSN-Framework.svg)](https://www.nuget.org/packages/TIKSN-Framework)
[![TIKSN Framework NuGet Package Downloads](https://img.shields.io/nuget/dt/TIKSN-Framework)](https://www.nuget.org/packages/TIKSN-Framework)
[![Developed by Tigran (TIKSN) Torosyan](https://img.shields.io/badge/Developed%20by-Tigran%20%28TIKSN%29%20Torosyan-orange.svg)](https://tiksn.com/project/tiksn-framework/)
[![StandWithUkraine](https://raw.githubusercontent.com/vshymanskyy/StandWithUkraine/main/badges/StandWithUkraine.svg)](https://github.com/vshymanskyy/StandWithUkraine/blob/main/docs/README.md)

## Project Description and Purpose

TIKSN Framework is a comprehensive, modern, and forward-looking .NET 10 framework designed for building a wide range of applications. It is a general-purpose toolkit that provides a rich set of features to accelerate application development, with a focus on modularity, dependency injection, and modern development practices.

## Major Functionality Provided

- **Versioning**: Provides robust versioning capabilities for your applications and components.
- **Finance**: Includes types and services for financial calculations, including `Money`, `Currency`, and `Foreign Exchange`.
- **Telemetry**: Offers a telemetry system for gathering and reporting analytics.
- **Licensing**: Provides a licensing mechanism to protect your software.
- **Progress Tracking**: Implements `Composite Weighted Progress` for tracking progress of complex operations.
- **Data Access**: Implements the Repository and Unit of Work patterns with support for:
    - Entity Framework Core
    - Azure Storage (Table and Blob)
    - MongoDB
    - LiteDB
    - RavenDB
- **Localization**: A robust localization system with support for language and region-specific resources.
- **Serialization**: Support for JSON, MessagePack, and Protocol Buffers.
- **Networking**: Includes a REST client (`Rest Requester`) and a RESTful repository implementation.
- **Dependency Injection**: Integrated with Autofac and `Microsoft.Extensions.DependencyInjection`, with base classes for Composition Root setup.
- **Shell Applications**: A powerful engine for creating sophisticated command-line (shell) applications.
- **Cross-Platform UI**: Support for building cross-platform mobile and desktop applications using .NET MAUI.

## Use Cases

The framework can be used to build:

- **Sophisticated Command-Line Applications**: The `ShellCommands` example demonstrates a powerful shell engine for creating complex command-line tools.
- **Cross-Platform Mobile and Desktop Apps**: The `TIKSN.Framework.Maui` project enables the development of applications for Android, iOS, macOS, and Windows from a single codebase.
- **Cloud-Native Applications**: With support for Azure storage and various databases, the framework is well-suited for building applications that run in the cloud.
- **Data-Driven Applications**: The extensive database support makes it a good choice for applications that need to interact with different data sources.

## Setup for Web Application

```csharp
using Autofac;
using Autofac.Extensions.DependencyInjection;
using TIKSN.DependencyInjection;
using TIKSN.Mapping;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());

builder.Services.AddFrameworkCore();

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

## Setup for Generic Application

```csharp
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TIKSN.DependencyInjection;

var hostBuilder = Host.CreateDefaultBuilder(args)
    .UseServiceProviderFactory(new AutofacServiceProviderFactory())
    .ConfigureServices((hostContext, services) =>
    {
        services.AddFrameworkCore();
        // Register your application specific services
    })
    .ConfigureContainer<ContainerBuilder>(containerBuilder =>
    {
        containerBuilder.RegisterModule<CoreModule>();
        // Optional: Register your application specific modules
    });

var host = hostBuilder.Build();

await host.RunAsync();
```
