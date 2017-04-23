using AutoMapper;
using Common_Models;
using Web_Service.Data.Entities;

namespace Web_Service.Mappers
{
	public class CurrencyMappingProfile : Profile
    {
		public CurrencyMappingProfile()
		{
			CreateMap<CurrencyModel, CurrencyEntity>();
			CreateMap<CurrencyEntity, CurrencyModel>();
		}
    }
}
