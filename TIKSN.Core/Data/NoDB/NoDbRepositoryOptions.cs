namespace TIKSN.Data.NoDB
{
    public class NoDbRepositoryOptions
    {
        public string ProjectId { get; set; }
    }

    public class NoDbRepositoryOptions<T> : NoDbRepositoryOptions
    {
    }
}
