using Common_Models;
using Console_Client.Rest;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
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

			var cultures = await _culturesRestRepository.GetAllAsync();

			Console.WriteLine($"There are {cultures.Count()} cultures in database.");

			var enUS = await _culturesRestRepository.GetByNameAsync("en-US");

			enUS.NativeName = "English (US)";

			await _culturesRestRepository.UpdateAsync(enUS);

			Console.Write("Delete? ");

			if (Console.ReadLine().StartsWith("y"))
				foreach (var culture in cultures)
					await _culturesRestRepository.RemoveAsync(culture);
		}

		static void Main(string[] args)
		{
			var compositionRootSetup = new CompositionRootSetup();
			var serviceProvider = compositionRootSetup.CreateServiceProvider();

			try
			{
				serviceProvider.GetRequiredService<Program>().ExecuteAsync().GetAwaiter().GetResult();
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
			}

			Console.WriteLine("Execution completed!");
			Console.ReadLine();
		}
	}
}