using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Web_Service.Data.Repositories;
using Common_Models;
using AutoMapper;
using Web_Service.Data.Entities;
using TIKSN.Data;

namespace Web_Service.Controllers
{
	[Route("api/[controller]")]
	public class CurrenciesController : Controller
	{
		private readonly ICurrencyRepository currencyRepository;
		private readonly IUnitOfWorkFactory unitOfWorkFactory;
		private readonly IMapper mapper;
		public CurrenciesController(ICurrencyRepository currencyRepository, IUnitOfWorkFactory unitOfWorkFactory, IMapper mapper)
		{
			this.currencyRepository = currencyRepository;
			this.unitOfWorkFactory = unitOfWorkFactory;
			this.mapper = mapper;
		}

		[HttpGet]
		public async Task<IEnumerable<CurrencyModel>> Get()
		{
			var entities = await currencyRepository.GetAllAsync();

			return mapper.Map<IEnumerable<CurrencyModel>>(entities);
		}

		// GET api/values/5
		[HttpGet("{id}")]
		public string Get(int id)
		{
			return "value";
		}

		// POST api/values
		[HttpPost]
		public async Task Post([FromBody]CurrencyModel model)
		{
			using (var unitOfWork = unitOfWorkFactory.Create())
			{
				var entity = mapper.Map<CurrencyEntity>(model);

				await currencyRepository.AddAsync(entity);

				await unitOfWork.CompleteAsync();
			}
		}

		// PUT api/values/5
		[HttpPut("{id}")]
		public void Put(int id, [FromBody]string value)
		{
		}

		// DELETE api/values/5
		[HttpDelete("{id}")]
		public void Delete(int id)
		{
		}
	}
}
