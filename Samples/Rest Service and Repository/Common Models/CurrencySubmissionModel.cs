namespace Common_Models
{
	public class CurrencySubmissionModel
    {
		public string Code { get; set; }
		public int Number { get; set; }
		public string CurrencySymbol { get; set; }
		public bool IsCurrent { get; set; }
		public string IsFund { get; set; }
	}
}
