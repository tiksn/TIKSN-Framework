<Query Kind="Program">
  <Output>DataGrids</Output>
  <NuGetReference Prerelease="true">TIKSN-Framework</NuGetReference>
  <NuGetReference>Microsoft.Extensions.Hosting</NuGetReference>
  <Namespace>Autofac</Namespace>
  <Namespace>Autofac.Core</Namespace>
  <Namespace>Autofac.Extensions.DependencyInjection</Namespace>
  <Namespace>Microsoft.Extensions.Configuration</Namespace>
  <Namespace>Microsoft.Extensions.DependencyInjection</Namespace>
  <Namespace>Microsoft.Extensions.Hosting</Namespace>
  <Namespace>System.Windows</Namespace>
  <Namespace>TIKSN.Configuration</Namespace>
  <Namespace>TIKSN.DependencyInjection</Namespace>
</Query>

void Main()
{
	var serviceProvider = Host.CreateDefaultBuilder()
	    .UseServiceProviderFactory(new AutofacServiceProviderFactory())
	    .ConfigureContainer<ContainerBuilder>(builder =>
	    {
	        builder.RegisterModule<CoreModule>();
	        builder.RegisterModule<PlatformModule>();
	    })
	    .ConfigureServices(services =>
	    {
	        services.AddFrameworkPlatform();
	    })
	    .Build().Services;

	var rng = serviceProvider.GetRequiredService<Random>();

	var randomNumber = rng.Next();

	Clipboard.SetData(DataFormats.UnicodeText, randomNumber.ToString());

	randomNumber.Dump();
}
