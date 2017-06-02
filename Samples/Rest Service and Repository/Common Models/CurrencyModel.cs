using TIKSN.Data;

namespace Common_Models
{
	public class CurrencyModel : IEntity<int>
	{
		public int ID { get; set; }
		public string Code { get; set; }
		public int Number { get; set; }
		public string CurrencySymbol { get; set; }
		public bool IsCurrent { get; set; }
		public bool IsFund { get; set; }
	}
}
