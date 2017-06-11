using System;

namespace TIKSN.Web.Rest
{
	[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
	public class AcceptLanguageAttribute : Attribute
	{
		public AcceptLanguageAttribute()
		{
			Quality = null;
		}

		public AcceptLanguageAttribute(double quality)
		{
			Quality = quality;
		}

		public double? Quality { get; }
	}
}