using Autofac;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using TIKSN.DependencyInjection;
using TIKSN.Localization;

namespace SelectingLocalization
{
	public class CompositionRoot : AutofacCompositionRootSetupBase
	{
		protected override void ConfigureContainerBuilder(ContainerBuilder builder)
		{
			builder.RegisterType<TextLocalizer>().Named<IStringLocalizer>("service");
			builder.RegisterDecorator<IStringLocalizer>((c, inner) => new StringLocalizerSelector(inner), "service").As<IStringLocalizer>().As<ILocalizationSelector>().SingleInstance();
		}

		protected override void ConfigureOptions(IServiceCollection services, IConfigurationRoot configuration)
		{
		}

		protected override void ConfigureServices(IServiceCollection services)
		{
		}
	}
}
