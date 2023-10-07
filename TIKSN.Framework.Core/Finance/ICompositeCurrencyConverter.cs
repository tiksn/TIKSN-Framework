namespace TIKSN.Finance
{
    public interface ICompositeCurrencyConverter : ICurrencyConverter
    {
        void Add(ICurrencyConverter converter);
    }
}
