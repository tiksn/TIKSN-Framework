using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Common_Models;
using Microsoft.AspNetCore.Mvc;
using Web_Service.Data.Repositories;
using TIKSN.Data;
using Web_Service.Data.Entities;

namespace Web_Service.Controllers
{
    [Route("api/[controller]")]
    public class RegionsController : Controller
    {
        private readonly IRegionRepository regionRepository;
        private readonly IUnitOfWorkFactory unitOfWorkFactory;
        private readonly IMapper _mapper;

        public RegionsController(IRegionRepository regionRepository, IUnitOfWorkFactory unitOfWorkFactory, IMapper mapper)
        {
            this.regionRepository = regionRepository;
            this.unitOfWorkFactory = unitOfWorkFactory;
            _mapper = mapper;
        }

        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        [HttpGet("{id}")]
        public async Task<RegionModel> Get(int id)
        {
            if (id != 0)
                throw new ArgumentException();

            var entity = await regionRepository.GetAsync(id);

            return _mapper.Map<RegionModel>(entity);
        }

        [HttpPost]
        public async Task Post([FromBody]RegionModel model)
        {
            if (model.Id != 0)
                throw new ArgumentException();

            using (var unitOfWork = unitOfWorkFactory.Create())
            {
                var entity = _mapper.Map<RegionEntity>(model);

                await regionRepository.AddAsync(entity);

                await unitOfWork.CompleteAsync();
            }
        }

        [HttpPut("{id}")]
        public async Task Put(int id, [FromBody]RegionModel model)
        {
            if (id != 0)
                throw new ArgumentException();

            if (model.Id != 0 && model.Id != id)
                throw new ArgumentException();

            using (var unitOfWork = unitOfWorkFactory.Create())
            {
                var entity = _mapper.Map<RegionEntity>(model);
                entity.Id = id;

                await regionRepository.UpdateAsync(entity);

                await unitOfWork.CompleteAsync();
            }
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public async Task Delete(int id)
        {
            if (id != 0)
                throw new ArgumentException();

            using (var unitOfWork = unitOfWorkFactory.Create())
            {
                var entity = await regionRepository.GetAsync(id);

                await regionRepository.RemoveAsync(entity);

                await unitOfWork.CompleteAsync();
            }
        }
    }
}
