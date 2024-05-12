using TIKSN.Mapping;

namespace TIKSN.Identity;

public class MapperIdentityGenerator<TSource, TDestination> : IIdentityGenerator<TDestination>
{
    private readonly IIdentityGenerator<TSource> identityGenerator;
    private readonly IMapper<TSource, TDestination> mapper;

    public MapperIdentityGenerator(
        IMapper<TSource, TDestination> mapper,
        IIdentityGenerator<TSource> identityGenerator)
    {
        this.mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        this.identityGenerator = identityGenerator ?? throw new ArgumentNullException(nameof(identityGenerator));
    }

    public TDestination Generate()
        => this.mapper.Map(this.identityGenerator.Generate());
}
