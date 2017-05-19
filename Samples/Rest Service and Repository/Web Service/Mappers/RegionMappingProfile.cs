using AutoMapper;
using Common_Models;
using Web_Service.Data.Entities;

namespace Web_Service.Mappers
{
    public class RegionMappingProfile : Profile
    {
        public RegionMappingProfile()
        {
            CreateMap<RegionEntity, RegionModel>();
            CreateMap<RegionModel, RegionEntity>();
        }
    }
}
