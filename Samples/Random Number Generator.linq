<Query Kind="Program">
  <Output>DataGrids</Output>
  <NuGetReference Prerelease="true">TIKSN-Framework</NuGetReference>
  <Namespace>Autofac</Namespace>
  <Namespace>Autofac.Core</Namespace>
  <Namespace>Microsoft.Extensions.Configuration</Namespace>
  <Namespace>Microsoft.Extensions.DependencyInjection</Namespace>
  <Namespace>System.Collections.Generic</Namespace>
  <Namespace>System.Linq</Namespace>
  <Namespace>TIKSN.Configuration</Namespace>
  <Namespace>TIKSN.DependencyInjection</Namespace>
</Query>

void Main()
{
	var serviceProvider = new CompositionRootSetup(new ConfigurationRootSetup().GetConfigurationRoot()).CreateServiceProvider();
	var rng = serviceProvider.GetRequiredService<Random>();
	
	var randomNumber = rng.Next();
	
	randomNumber.Dump();
}

public class CompositionRootSetup : AutofacPlatformCompositionRootSetupBase 
{
	public CompositionRootSetup(IConfigurationRoot configuration) : base(configuration) {}
	
	protected override void ConfigureContainerBuilder(ContainerBuilder builder)
	{
	}
	
	protected override void ConfigureOptions(IServiceCollection services, IConfigurationRoot configuration)
	{
	}
}

public class ConfigurationRootSetup : ConfigurationRootSetupBase
{
}