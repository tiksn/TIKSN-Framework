using AutoMapper;
using Common_Models;
using Web_Service.Data.Entities;

namespace Web_Service.Mappers
{
    public class CultureMappingProfile : Profile
    {
        public CultureMappingProfile()
        {
            CreateMap<CultureModel, CultureEntity>();
            CreateMap<CultureEntity, CultureModel>();
        }
    }
}
