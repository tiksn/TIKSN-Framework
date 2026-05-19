using TIKSN.Time;

namespace TIKSN.Finance.Pricing;

public interface IDailyPricing : IPricing
{
    public IDay Day { get; }
}

public interface IDailyPricing<TDay> : IDailyPricing
    where TDay : IDay<TDay>
{
    public new TDay Day { get; }
}
