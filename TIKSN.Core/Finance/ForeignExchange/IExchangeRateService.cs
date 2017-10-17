using System.Threading.Tasks;

namespace TIKSN.Finance.ForeignExchange
{
	public interface IExchangeRateService
	{
		Task InitializeAsync();
	}
}
