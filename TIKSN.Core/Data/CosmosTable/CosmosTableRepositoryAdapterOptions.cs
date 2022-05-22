namespace TIKSN.Data.CosmosTable
{
    public class CosmosTableRepositoryAdapterOptions
    {
        public enum AddOptions { Add, AddOrMerge, AddOrReplace }

        public enum UpdateOptions { Merge, Replace }

        public CosmosTableRepositoryAdapterOptions() => this.AddOption = AddOptions.Add;

        public AddOptions AddOption { get; set; }

        public UpdateOptions UpdateOption { get; set; }
    }
}
