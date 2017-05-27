using Common_Models;
using Console_Client.Rest;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Globalization;
using System.Threading.Tasks;
using TIKSN.Shell;

namespace Console_Client
{
	class Program : IShellCommand
	{
		private readonly ICulturesRestRepository _culturesRestRepository;

		public Program(ICulturesRestRepository culturesRestRepository)
		{
			_culturesRestRepository = culturesRestRepository;
		}

		public async Task ExecuteAsync()
		{
			await _culturesRestRepository.AddAsync(new CultureModel
			{
				Code = "en",
				EnglishName = "English",
				Lcid = 123,
				NativeName = "English",
				ParentId = null,
				RegionId = null
			});

			await _culturesRestRepository.AddAsync(new CultureModel
			{
				Code = "en-US",
				EnglishName = "English (US)",
				Lcid = 123,
				NativeName = "English (United States)",
				ParentId = null,
				RegionId = null
			});
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