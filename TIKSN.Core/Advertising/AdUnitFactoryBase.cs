using System.Collections.Generic;

namespace TIKSN.Advertising
{
    public abstract class AdUnitFactoryBase : IAdUnitFactory
    {
        protected readonly Dictionary<string, AdUnitBundle> _adUnitBundles;
        private readonly IAdUnitSelector _adUnitSelector;

        protected AdUnitFactoryBase(IAdUnitSelector adUnitSelector)
        {
            this._adUnitSelector = adUnitSelector;
            this._adUnitBundles = new Dictionary<string, AdUnitBundle>();

            this.Register();
        }

        public AdUnit Create(string key)
        {
            var adUnitBundle = this._adUnitBundles[key];

            return this._adUnitSelector.Select(adUnitBundle);
        }

        protected abstract void Register();
    }
}
