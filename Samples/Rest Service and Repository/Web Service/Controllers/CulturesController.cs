using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Web_Service.Data.Repositories;
using AutoMapper;
using Common_Models;
using TIKSN.Data;
using Web_Service.Data.Entities;

namespace Web_Service.Controllers
{
    [Route("api/[controller]")]
    public class CulturesController : Controller
    {
        private readonly ICultureRepository cultureRepository;
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;
        private readonly IMapper _mapper;

        public CulturesController(ICultureRepository cultureRepository, IUnitOfWorkFactory unitOfWorkFactory, IMapper mapper)
        {
            this.cultureRepository = cultureRepository;
            _unitOfWorkFactory = unitOfWorkFactory;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IEnumerable<CultureModel>> Get()
        {
            var entities = await cultureRepository.GetAllAsync();

            return _mapper.Map<IEnumerable<CultureModel>>(entities);
        }

        [HttpGet("{id}")]
        public async Task<CultureModel> Get(int id)
        {
            var entity = await cultureRepository.GetAsync(id);

            return _mapper.Map<CultureModel>(entity);
        }

        [HttpPost]
        public async Task Post([FromBody]CultureSubmissionModel model)
        {
            using (var unitOfWork = _unitOfWorkFactory.Create())
            {
                var entity = _mapper.Map<CultureEntity>(model);

                await cultureRepository.AddAsync(entity);

                await unitOfWork.CompleteAsync();
            }
        }

        [HttpPut("{id}")]
        public async Task Put(int id, [FromBody]CultureSubmissionModel model)
        {
            using (var unitOfWork = _unitOfWorkFactory.Create())
            {
                var entity = _mapper.Map<CultureEntity>(model);
                entity.Id = id;

                await cultureRepository.UpdateAsync(entity);

                await unitOfWork.CompleteAsync();
            }
        }

        [HttpDelete("{id}")]
        public async Task Delete(int id)
        {
            using (var unitOfWork = _unitOfWorkFactory.Create())
            {
                var entity = await cultureRepository.GetAsync(id);

                await cultureRepository.RemoveAsync(entity);

                await unitOfWork.CompleteAsync();
            }
        }
    }
}
