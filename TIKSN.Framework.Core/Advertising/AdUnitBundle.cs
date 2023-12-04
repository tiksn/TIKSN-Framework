namespace TIKSN.Advertising;

public class AdUnitBundle
{
    public AdUnitBundle(AdUnit designTime, AdUnit production)
    {
        this.DesignTime = designTime ?? throw new ArgumentNullException(nameof(designTime));
        this.Production = production ?? throw new ArgumentNullException(nameof(production));

        if (!designTime.IsTest)
        {
            throw new ArgumentException($"Value of {nameof(designTime)}.{nameof(designTime.IsTest)} must be true.",
                nameof(designTime));
        }

        if (production.IsTest)
        {
            throw new ArgumentException($"Value of {nameof(production)}.{nameof(production.IsTest)} must be false.",
                nameof(production));
        }
    }

    public AdUnit DesignTime { get; }

    public AdUnit Production { get; }
}
