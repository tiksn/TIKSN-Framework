namespace Common_Models
{
	public class CultureModel
    {
		public int Id { get; set; }
		public string Code { get; set; }
		public int? ParentId { get; set; }
		public int Lcid { get; set; }
		public string EnglishName { get; set; }
		public string NativeName { get; set; }
		public int? RegionId { get; set; }
	}
}
