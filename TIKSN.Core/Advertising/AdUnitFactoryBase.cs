using System.Collections.Generic;

namespace TIKSN.Advertising
{
    public abstract class AdUnitFactoryBase : IAdUnitFactory
    {
        protected readonly Dictionary<string, AdUnitBundle> _adUnitBundles;
        private readonly IAdUnitSelector _adUnitSelector;

        protected AdUnitFactoryBase(IAdUnitSelector adUnitSelector)
        {
            _adUnitSelector = adUnitSelector;
            _adUnitBundles = new Dictionary<string, AdUnitBundle>();

            Register();
        }

        public AdUnit Create(string key)
        {
            var adUnitBundle = _adUnitBundles[key];

            return _adUnitSelector.Select(adUnitBundle);
        }

        protected abstract void Register();
    }
}