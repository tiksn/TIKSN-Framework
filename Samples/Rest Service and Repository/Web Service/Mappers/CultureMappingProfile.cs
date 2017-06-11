using AutoMapper;
using Common_Models;
using Web_Service.Data.Entities;

namespace Web_Service.Mappers
{
    public class CultureMappingProfile : Profile
    {
        public CultureMappingProfile()
        {
            CreateMap<CultureModel, CultureEntity>().ForMember(m => m.Id, c => c.MapFrom(x => x.ID));
            CreateMap<CultureEntity, CultureModel>().ForMember(m => m.ID, c => c.MapFrom(x => x.Id));
        }
    }
}
