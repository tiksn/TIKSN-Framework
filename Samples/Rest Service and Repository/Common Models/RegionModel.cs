﻿namespace Common_Models
{
	public class RegionModel
    {
		public int Id { get; set; }
		public string Code { get; set; }
		public bool GeoId { get; set; }
		public int CurrencyId { get; set; }
		public string EnglishName { get; set; }
		public string NativeName { get; set; }
	}
}
