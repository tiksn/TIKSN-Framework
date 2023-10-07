using IdGen;

namespace TIKSN.Identity;

public class IdGenIdentityGenerator : IIdentityGenerator<long>
{
    private readonly IIdGenerator<long> idGenerator;

    public IdGenIdentityGenerator(IIdGenerator<long> idGenerator)
    {
        this.idGenerator = idGenerator ?? throw new ArgumentNullException(nameof(idGenerator));
    }

    public long Generate()
    {
        return idGenerator.CreateId();
    }
}
