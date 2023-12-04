using TIKSN.Finance.ForeignExchange.Data;
using TIKSN.Finance.ForeignExchange.Data.LiteDB;
using TIKSN.Mapping;

namespace TIKSN.Finance.ForeignExchange.LiteDB;

public class DataEntityMapper
    : IMapper<ExchangeRateEntity, ExchangeRateDataEntity>
    , IMapper<ExchangeRateDataEntity, ExchangeRateEntity>
    , IMapper<ForeignExchangeEntity, ForeignExchangeDataEntity>
    , IMapper<ForeignExchangeDataEntity, ForeignExchangeEntity>
{
    public ExchangeRateDataEntity Map(ExchangeRateEntity source)
    {
        ArgumentNullException.ThrowIfNull(source);

        return new ExchangeRateDataEntity()
        {
            Rate = source.Rate,
            AsOn = source.AsOn,
            ID = source.ID,
            BaseCurrencyCode = source.BaseCurrencyCode,
            CounterCurrencyCode = source.CounterCurrencyCode,
            ForeignExchangeID = source.ForeignExchangeID,
        };
    }

    public ExchangeRateEntity Map(ExchangeRateDataEntity source)
    {
        ArgumentNullException.ThrowIfNull(source);

        return new ExchangeRateEntity(
            source.ID,
            source.BaseCurrencyCode,
            source.CounterCurrencyCode,
            source.ForeignExchangeID,
            source.AsOn,
            source.Rate);
    }

    public ForeignExchangeDataEntity Map(ForeignExchangeEntity source)
    {
        ArgumentNullException.ThrowIfNull(source);

        return new ForeignExchangeDataEntity()
        {
            ID = source.ID,
            CountryCode = source.CountryCode,
            LongNameKey = source.LongNameKey,
            ShortNameKey = source.ShortNameKey,
        };
    }

    public ForeignExchangeEntity Map(ForeignExchangeDataEntity source)
    {
        ArgumentNullException.ThrowIfNull(source);

        return new ForeignExchangeEntity(
            source.ID,
            source.CountryCode,
            source.LongNameKey,
            source.ShortNameKey);
    }
}
