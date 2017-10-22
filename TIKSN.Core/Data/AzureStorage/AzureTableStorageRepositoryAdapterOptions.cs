namespace TIKSN.Data.AzureStorage
{
    public class AzureTableStorageRepositoryAdapterOptions
    {
        public AzureTableStorageRepositoryAdapterOptions()
        {
            AddOption = AddOptions.Add;
        }

        public enum AddOptions { Add, AddOrMerge, AddOrReplace }

        public enum UpdateOptions { Merge, Replace }

        public AddOptions AddOption { get; set; }

        public UpdateOptions UpdateOption { get; set; }
    }
}