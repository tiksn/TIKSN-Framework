namespace TIKSN.Data.RavenDB;

public class RavenUnitOfWorkFactoryOptions
{
    public IReadOnlyList<string> Urls { get; set; } = [];

    public string Database { get; set; } = string.Empty;
}
