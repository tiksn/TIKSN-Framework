namespace TIKSN.Data.RavenDB;

public class RavenUnitOfWorkFactoryOptions
{
    public string Database { get; set; } = string.Empty;
    public IReadOnlyList<string> Urls { get; set; } = [];
}
