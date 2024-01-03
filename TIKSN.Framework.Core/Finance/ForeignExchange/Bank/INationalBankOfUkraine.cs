namespace TIKSN.Finance.ForeignExchange.Bank;

/// <summary>
///     Exchange rate converter of National Bank of Ukraine
/// </summary>
/// <seealso cref="ICurrencyConverter" />
public interface INationalBankOfUkraine : ICurrencyConverter, IExchangeRatesProvider;
