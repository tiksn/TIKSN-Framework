using System;
using AutoMapper;

namespace TIKSN.Data
{
    public class AutoMapperAdapter<TSource, TDestination> : IMapper<TSource, TDestination>
    {
        private readonly IMapper mapper;

        public AutoMapperAdapter(IMapper mapper)
        {
            this.mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public TDestination Map(TSource source) => this.mapper.Map<TDestination>(source);
    }
}
