using TIKSN.Time;

namespace TIKSN.Finance.Pricing;

public interface IMonthlyPricing : IPricing
{
    public IMonth Month { get; }
}

public interface IMonthlyPricing<TMonth> : IMonthlyPricing
    where TMonth : IMonth<TMonth>
{
    public new TMonth Month { get; }
}
