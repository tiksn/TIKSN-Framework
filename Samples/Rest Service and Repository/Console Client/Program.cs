using System;

namespace Console_Client
{
	class Program
	{
		static void Main(string[] args)
		{
			var compositionRootSetup = new CompositionRootSetup();
			var serviceProvider = compositionRootSetup.CreateServiceProvider();

			Console.ReadLine();
		}
	}
}