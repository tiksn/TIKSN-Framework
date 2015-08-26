using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TIKSN.Finance.PricingStrategy
{
	public class PsychologicalPricingStrategy : IPricingStrategy
	{
		public Money EstimateMarketPrice(Money basePrice)
		{
			var estimatedPrice = EstimateMarketPrice(basePrice.Amount);

			return new Money(basePrice.Currency, estimatedPrice);
		}

		public decimal EstimateMarketPrice(decimal basePrice)
		{
			throw new NotImplementedException();
		}
	}
}
