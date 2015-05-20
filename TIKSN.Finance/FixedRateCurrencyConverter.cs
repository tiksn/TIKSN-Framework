namespace TIKSN.Finance
{
	public class FixedRateCurrencyConverter : ICurrencyConverter
	{
		private CurrencyPair pair;
		private decimal Rate;

		public FixedRateCurrencyConverter(CurrencyPair Pair, decimal Rate)
		{
			this.CurrencyPair = Pair;

			if (Rate > decimal.Zero)
			{
				this.Rate = Rate;
			}
			else
			{
				throw new System.ArgumentException("Rate cannot be negative or zero.", "Rate");
			}
		}

		public CurrencyPair CurrencyPair
		{
			get
			{
				return this.pair;
			}
			private set
			{
				if (object.ReferenceEquals(value, null))
					throw new System.ArgumentNullException();

				this.pair = value;
			}
		}

		public Money ConvertCurrency(Money BaseMoney, CurrencyInfo CounterCurrency, System.DateTime asOn)
		{
			CurrencyPair requiredPair = new CurrencyPair(BaseMoney.Currency, CounterCurrency);

			if (this.CurrencyPair == requiredPair)
			{
				return new Money(this.CurrencyPair.CounterCurrency, BaseMoney.Amount * this.Rate);
			}
			else
			{
				throw new System.ArgumentException("Unsupported currency pair.");
			}
		}

		public System.Collections.Generic.IEnumerable<CurrencyPair> GetCurrencyPairs(System.DateTime asOn)
		{
			yield return this.pair;
		}

		public decimal GetExchangeRate(CurrencyPair Pair, System.DateTime asOn)
		{
			if (this.CurrencyPair == Pair)
			{
				return this.Rate;
			}
			else
			{
				throw new System.ArgumentException("Unsupported currency pair.");
			}

			throw new System.NotImplementedException();
		}
	}
}