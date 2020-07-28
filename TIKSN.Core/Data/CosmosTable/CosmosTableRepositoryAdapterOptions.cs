namespace TIKSN.Data.CosmosTable
{
    public class CosmosTableRepositoryAdapterOptions
    {
        public CosmosTableRepositoryAdapterOptions()
        {
            AddOption = AddOptions.Add;
        }

        public enum AddOptions { Add, AddOrMerge, AddOrReplace }

        public enum UpdateOptions { Merge, Replace }

        public AddOptions AddOption { get; set; }

        public UpdateOptions UpdateOption { get; set; }
    }
}