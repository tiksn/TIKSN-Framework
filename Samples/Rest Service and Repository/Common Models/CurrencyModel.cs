using TIKSN.Data;

namespace Common_Models
{
	public class CurrencyModel : Entity<int>
	{
		public string Code { get; set; }
		public int Number { get; set; }
		public string CurrencySymbol { get; set; }
		public bool IsCurrent { get; set; }
		public bool IsFund { get; set; }
	}
}
