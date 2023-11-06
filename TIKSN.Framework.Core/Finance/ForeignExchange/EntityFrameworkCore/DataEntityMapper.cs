using TIKSN.Finance.ForeignExchange.Data;
using TIKSN.Finance.ForeignExchange.Data.EntityFrameworkCore;
using TIKSN.Mapping;

namespace TIKSN.Finance.ForeignExchange.EntityFrameworkCore;

public class DataEntityMapper
    : IMapper<ExchangeRateEntity, ExchangeRateDataEntity>
    , IMapper<ExchangeRateDataEntity, ExchangeRateEntity>
    , IMapper<ForeignExchangeEntity, ForeignExchangeDataEntity>
    , IMapper<ForeignExchangeDataEntity, ForeignExchangeEntity>
{
    public ExchangeRateDataEntity Map(ExchangeRateEntity source)
    {
        if (source == null)
        {
            throw new ArgumentNullException(nameof(source));
        }

        return new ExchangeRateDataEntity()
        {
            ForeignExchange = null,
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
        if (source == null)
        {
            throw new ArgumentNullException(nameof(source));
        }

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
        if (source == null)
        {
            throw new ArgumentNullException(nameof(source));
        }

        return new ForeignExchangeDataEntity()
        {
            ID = source.ID,
            CountryCode = source.CountryCode,
            ExchangeRates = null,
            LongNameKey = source.LongNameKey,
            ShortNameKey = source.ShortNameKey,
        };
    }

    public ForeignExchangeEntity Map(ForeignExchangeDataEntity source)
    {
        if (source == null)
        {
            throw new ArgumentNullException(nameof(source));
        }

        return new ForeignExchangeEntity(
            source.ID,
            source.CountryCode,
            source.LongNameKey,
            source.ShortNameKey);
    }
}
