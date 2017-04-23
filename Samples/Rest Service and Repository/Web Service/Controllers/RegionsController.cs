using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Web_Service.Data.Repositories;

namespace Web_Service.Controllers
{
	[Route("api/[controller]")]
	public class RegionsController : Controller
	{
		private readonly IRegionRepository regionRepository;
		private readonly IUnitOfWorkFactory unitOfWorkFactory;

		public RegionsController(IRegionRepository regionRepository, IUnitOfWorkFactory unitOfWorkFactory)
		{
			this.regionRepository = regionRepository;
			this.unitOfWorkFactory = unitOfWorkFactory;
		}

		// GET api/values
		[HttpGet]
		public IEnumerable<string> Get()
		{
			return new string[] { "value1", "value2" };
		}

		// GET api/values/5
		[HttpGet("{id}")]
		public string Get(int id)
		{
			return "value";
		}

		// POST api/values
		[HttpPost]
		public async Task Post([FromBody]string value)
		{
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
