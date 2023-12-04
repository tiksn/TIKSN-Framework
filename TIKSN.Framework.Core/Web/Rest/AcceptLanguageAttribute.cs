namespace TIKSN.Web.Rest;

[AttributeUsage(AttributeTargets.Property)]
public class AcceptLanguageAttribute : Attribute
{
    public AcceptLanguageAttribute() => this.Quality = null;

    public AcceptLanguageAttribute(double quality) => this.Quality = quality;

    public double? Quality { get; }
}
