namespace TIKSN.Advertising
{
    public abstract class AdUnitFactoryBase : IAdUnitFactory
    {
        private readonly Dictionary<string, AdUnitBundle> adUnitBundles;
        private readonly IAdUnitSelector adUnitSelector;

        protected AdUnitFactoryBase(IAdUnitSelector adUnitSelector)
        {
            this.adUnitSelector = adUnitSelector;
            this.adUnitBundles = new Dictionary<string, AdUnitBundle>();

            this.Register();
        }

        public AdUnit Create(string key)
        {
            var adUnitBundle = this.adUnitBundles[key];

            return this.adUnitSelector.SelectAdUnit(adUnitBundle);
        }

        protected abstract void Register();
    }
}
