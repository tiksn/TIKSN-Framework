using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;
using TIKSN.Shell;

namespace Console_Client
{
	class Program : IShellCommand
	{
		public Program()
		{

		}

		public async Task ExecuteAsync()
		{
		}

		static void Main(string[] args)
		{
			var compositionRootSetup = new CompositionRootSetup();
			var serviceProvider = compositionRootSetup.CreateServiceProvider();

			serviceProvider.GetRequiredService<Program>().ExecuteAsync().GetAwaiter().GetResult();

			Console.WriteLine("Execution completed!");
			Console.ReadLine();
		}
	}
}