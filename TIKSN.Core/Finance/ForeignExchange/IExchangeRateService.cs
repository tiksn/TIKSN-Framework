using System;
using System.Threading;
using System.Threading.Tasks;
using LanguageExt;

namespace TIKSN.Finance.ForeignExchange
{
    public interface IExchangeRateService
    {
        /// <summary>
        /// Converts Currency pairs
        /// </summary>
        /// <param name="baseMoney"></param>
        /// <param name="counterCurrency"></param>
        /// <param name="asOn"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<Option<Money>> ConvertCurrencyAsync(
            Money baseMoney,
            CurrencyInfo counterCurrency,
            DateTimeOffset asOn,
            CancellationToken cancellationToken);

        /// <summary>
        /// Converts Currency pairs allowing Double Conversion via Intermediary Currency
        /// </summary>
        /// <param name="baseMoney"></param>
        /// <param name="counterCurrency"></param>
        /// <param name="asOn"></param>
        /// <param name="intermediaryCurrency"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<Option<Money>> ConvertCurrencyAsync(
            Money baseMoney,
            CurrencyInfo counterCurrency,
            DateTimeOffset asOn,
            CurrencyInfo intermediaryCurrency,
            CancellationToken cancellationToken);

        /// <summary>
        /// Gets Exchange Rate
        /// </summary>
        /// <param name="pair"></param>
        /// <param name="asOn"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<Option<decimal>> GetExchangeRateAsync(
            CurrencyPair pair,
            DateTimeOffset asOn,
            CancellationToken cancellationToken);

        /// <summary>
        /// Gets Exchange Rate allowing Double Conversion via Intermediary Currency
        /// </summary>
        /// <param name="pair"></param>
        /// <param name="asOn"></param>
        /// <param name="intermediaryCurrency"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<Option<decimal>> GetExchangeRateAsync(
            CurrencyPair pair,
            DateTimeOffset asOn,
            CurrencyInfo intermediaryCurrency,
            CancellationToken cancellationToken);

        Task InitializeAsync(CancellationToken cancellationToken);
    }
}
