using TIKSN.Data;

namespace Common_Models
{
	public class RegionModel : IEntity<int>
	{
		public int ID { get; set; }
		public string Code { get; set; }
		public bool GeoId { get; set; }
		public int CurrencyId { get; set; }
		public string EnglishName { get; set; }
		public string NativeName { get; set; }
	}
}
