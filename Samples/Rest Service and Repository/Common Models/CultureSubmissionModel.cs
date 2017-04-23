namespace Common_Models
{
	public class CultureSubmissionModel
    {
		public string Code { get; set; }
		public int? ParentId { get; set; }
		public int Lcid { get; set; }
		public string EnglishName { get; set; }
		public string NativeName { get; set; }
		public int? RegionId { get; set; }
	}
}
