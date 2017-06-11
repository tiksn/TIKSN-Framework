using Microsoft.Extensions.DependencyInjection;
using TIKSN.Shell;

namespace Shell_Commander
{
	internal class Program
	{
		private static void Main(string[] args)
		{
			var composition = new CompositionRootSetup();

			var serviceProvider = composition.CreateServiceProvider();

			var engine = serviceProvider.GetRequiredService<IShellCommandEngine>();

			var thisAssembly = typeof(Program).Assembly;

			engine.AddAssembly(thisAssembly);

			engine.RunAsync().Wait();
		}
	}
}