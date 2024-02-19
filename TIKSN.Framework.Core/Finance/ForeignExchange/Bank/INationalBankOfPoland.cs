namespace TIKSN.Finance.ForeignExchange.Bank;

/// <summary>
///     Exchange rate converter of National Bank of Poland
/// </summary>
/// <seealso cref="ICurrencyConverter" />
public interface INationalBankOfPoland : ICurrencyConverter, IExchangeRatesProvider;
