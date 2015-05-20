namespace TIKSN.Finance
{
	public interface ICurrencyConverter
	{
		Money ConvertCurrency(Money BaseMoney, CurrencyInfo CounterCurrency, System.DateTime asOn);

		System.Collections.Generic.IEnumerable<CurrencyPair> GetCurrencyPairs(System.DateTime asOn);

		decimal GetExchangeRate(CurrencyPair Pair, System.DateTime asOn);
	}
}