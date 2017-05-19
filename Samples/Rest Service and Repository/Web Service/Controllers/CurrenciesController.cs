using System;
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

        [HttpGet("{id}")]
        public async Task<CurrencyModel> Get(int id)
        {
            if (id != 0)
                throw new ArgumentException();

            var entity = await currencyRepository.GetAsync(id);

            return mapper.Map<CurrencyModel>(entity);
        }

        [HttpPost]
        public async Task Post([FromBody]CurrencyModel model)
        {
            if (model.Id != 0)
                throw new ArgumentException();

            using (var unitOfWork = unitOfWorkFactory.Create())
            {
                var entity = mapper.Map<CurrencyEntity>(model);

                await currencyRepository.AddAsync(entity);

                await unitOfWork.CompleteAsync();
            }
        }

        [HttpPut("{id}")]
        public async Task Put(int id, [FromBody]CurrencyModel model)
        {
            if (id != 0)
                throw new ArgumentException();

            if (model.Id != 0 && model.Id != id)
                throw new ArgumentException();

            using (var unitOfWork = unitOfWorkFactory.Create())
            {
                var entity = mapper.Map<CurrencyEntity>(model);
                entity.Id = id;

                await currencyRepository.UpdateAsync(entity);

                await unitOfWork.CompleteAsync();
            }
        }

        [HttpDelete("{id}")]
        public async Task Delete(int id)
        {
            if (id != 0)
                throw new ArgumentException();

            using (var unitOfWork = unitOfWorkFactory.Create())
            {
                var entity = await currencyRepository.GetAsync(id);

                await currencyRepository.RemoveAsync(entity);

                await unitOfWork.CompleteAsync();
            }
        }
    }
}
