using AutoMapper;
using Microsoft.Extensions.DependencyInjection;

namespace TIKSN.Mapping
{
    public class MapperProfile : Profile
    {
        protected readonly IServiceCollection services;

        public MapperProfile(IServiceCollection services)
        {
            this.services = services ?? throw new ArgumentNullException(nameof(services));
        }

        public IMappingExpression<TSource, TDestination> AddMapper<TSource, TDestination>(MemberList memberList)
        {
            _ = this.services.AddTransient<IMapper<TSource, TDestination>, AutoMapperAdapter<TSource, TDestination>>();

            return this.CreateMap<TSource, TDestination>(memberList);
        }

        public IMappingExpression<TSource, TDestination> AddMapper<TSource, TDestination>()
        {
            _ = this.services.AddTransient<IMapper<TSource, TDestination>, AutoMapperAdapter<TSource, TDestination>>();

            return this.CreateMap<TSource, TDestination>();
        }
    }
}
