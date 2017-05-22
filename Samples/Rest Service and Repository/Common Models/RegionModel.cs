using TIKSN.Data;

namespace Common_Models
{
	public class RegionModel : Entity<int>
	{
		public string Code { get; set; }
		public bool GeoId { get; set; }
		public int CurrencyId { get; set; }
		public string EnglishName { get; set; }
		public string NativeName { get; set; }
	}
}
