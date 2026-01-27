namespace TIKSN.Finance;

public interface ICompositeCurrencyConverter : ICurrencyConverter
{
    public void Add(ICurrencyConverter converter);
}
