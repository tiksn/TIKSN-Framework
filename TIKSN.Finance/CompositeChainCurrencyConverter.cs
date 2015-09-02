using QuickGraph;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TIKSN.Finance
{
	public class CompositeChainCurrencyConverter : CompositeCurrencyConverter
	{
		public CompositeChainCurrencyConverter(ICurrencyConversionCompositionStrategy compositionStrategy)
			: base(compositionStrategy)
		{

		}

		public override Task<Money> ConvertCurrencyAsync(Money baseMoney, CurrencyInfo counterCurrency, DateTimeOffset asOn)
		{
			throw new NotImplementedException();
		}

		public override Task<IEnumerable<CurrencyPair>> GetCurrencyPairsAsync(DateTimeOffset asOn)
		{
			throw new NotImplementedException();
		}

		public override Task<decimal> GetExchangeRateAsync(CurrencyPair pair, DateTimeOffset asOn)
		{
			throw new NotImplementedException();
		}

		private async void GetConverterChain(DateTimeOffset asOn)
		{

		}

		private async Task<BidirectionalGraph<CurrencyInfo, TaggedEdge<CurrencyInfo, ICurrencyConverter>>> CreateGraph(DateTimeOffset asOn)
		{
			var graph = new BidirectionalGraph<CurrencyInfo, TaggedEdge<CurrencyInfo, ICurrencyConverter>>();

			foreach (var converter in this.converters)
			{
				var currencyPairs = await converter.GetCurrencyPairsAsync(asOn);

				foreach (var currencyPair in currencyPairs)
				{
					graph.AddVertex(currencyPair.BaseCurrency);
					graph.AddVertex(currencyPair.CounterCurrency);

					graph.AddVerticesAndEdge(new TaggedEdge<CurrencyInfo, ICurrencyConverter>(currencyPair.BaseCurrency, currencyPair.CounterCurrency, converter));
				}
			}

			return graph;
		}
	}
}
