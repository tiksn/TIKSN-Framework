using TIKSN.Time;

namespace TIKSN.Finance.Pricing;

public interface IYearlyPricing : IPricing
{
    public IYear Year { get; }
}

public interface IYearlyPricing<TYear> : IYearlyPricing
    where TYear : IYear<TYear>
{
    public new TYear Year { get; }
}
