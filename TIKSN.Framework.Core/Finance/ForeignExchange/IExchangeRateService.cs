using LanguageExt;

namespace TIKSN.Finance.ForeignExchange;

public interface IExchangeRateService
{
    /// <summary>
    /// Converts Currency pairs
    /// </summary>
    /// <param name="baseMoney">Initial Money</param>
    /// <param name="counterCurrency">Convert to Currency</param>
    /// <param name="asOn">Date and Time for which exchange rate will be requested</param>
    /// <param name="cancellationToken">Cancellation Token</param>
    /// <returns>Converted Money</returns>
    public Task<Option<Money>> ConvertCurrencyAsync(
        Money baseMoney,
        CurrencyInfo counterCurrency,
        DateTimeOffset asOn,
        CancellationToken cancellationToken);

    /// <summary>
    /// Converts Currency pairs allowing Double Conversion via Intermediary Currency
    /// </summary>
    /// <param name="baseMoney">Initial Money</param>
    /// <param name="counterCurrency">Convert to Currency</param>
    /// <param name="asOn">Date and Time for which exchange rate will be requested</param>
    /// <param name="intermediaryCurrency">Convert via Intermediary Currency</param>
    /// <param name="cancellationToken">Cancellation Token</param>
    /// <returns>Converted Money</returns>
    public Task<Option<Money>> ConvertCurrencyAsync(
        Money baseMoney,
        CurrencyInfo counterCurrency,
        DateTimeOffset asOn,
        CurrencyInfo intermediaryCurrency,
        CancellationToken cancellationToken);

    /// <summary>
    /// Gets Exchange Rate
    /// </summary>
    /// <param name="pair">Conversion Currency Pairs</param>
    /// <param name="asOn">Date and Time for which exchange rate will be requested</param>
    /// <param name="cancellationToken">Cancellation Token</param>
    /// <returns>Exchange Rate</returns>
    public Task<Option<decimal>> GetExchangeRateAsync(
        CurrencyPair pair,
        DateTimeOffset asOn,
        CancellationToken cancellationToken);

    /// <summary>
    /// Gets Exchange Rate allowing Double Conversion via Intermediary Currency
    /// </summary>
    /// <param name="pair">Conversion Currency Pairs</param>
    /// <param name="asOn">Date and Time for which exchange rates will be requested</param>
    /// <param name="intermediaryCurrency">Convert via Intermediary Currency</param>
    /// <param name="cancellationToken">Cancellation Token</param>
    /// <returns>Exchange Rate</returns>
    public Task<Option<decimal>> GetExchangeRateAsync(
        CurrencyPair pair,
        DateTimeOffset asOn,
        CurrencyInfo intermediaryCurrency,
        CancellationToken cancellationToken);

    public Task InitializeAsync(CancellationToken cancellationToken);
}
