namespace TIKSN.Data.CosmosTable;

public class CosmosTableRepositoryAdapterOptions
{
    public CosmosTableRepositoryAdapterOptions()
    {
        this.AddOption = AddOptions.Add;
        this.UpdateOption = UpdateOptions.Merge;
    }

    public enum AddOptions
    {
        Add = 0,
        AddOrMerge = 1,
        AddOrReplace = 2,
    }

    public enum UpdateOptions
    {
        Merge = 0,
        Replace = 1,
    }

    public AddOptions AddOption { get; set; }

    public UpdateOptions UpdateOption { get; set; }
}
