using System.Reflection;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Localization;
using ShellCommands;
using TIKSN.DependencyInjection;
using TIKSN.Shell;

var builder = Host.CreateDefaultBuilder(args);

builder.UseServiceProviderFactory(new AutofacServiceProviderFactory());

builder.ConfigureServices(services => services.AddFrameworkCore());

builder.ConfigureContainer<ContainerBuilder>(containerBuilder =>
{
    _ = containerBuilder.RegisterModule<CoreModule>();

    _ = containerBuilder.RegisterType<MainStringLocalizer>().As<IStringLocalizer>().SingleInstance();
});

var app = builder.Build();

var shellCommandEngine = app.Services.GetRequiredService<IShellCommandEngine>();

shellCommandEngine.AddAssembly(Assembly.GetExecutingAssembly());

await shellCommandEngine.RunAsync().ConfigureAwait(false);
