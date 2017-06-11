using TIKSN.Data;

namespace Common_Models
{
	public class CultureModel : IEntity<int>
    {
	    public int ID { get; set; }
	    public string Code { get; set; }
		public int? ParentId { get; set; }
		public int Lcid { get; set; }
		public string EnglishName { get; set; }
		public string NativeName { get; set; }
		public int? RegionId { get; set; }
	}
}
