using System;

namespace TIKSN.Advertising
{
    public class AdUnitBundle
    {
        public AdUnitBundle(AdUnit designTime, AdUnit tablet, AdUnit mobile)
        {
            Tablet = tablet;
            DesignTime = designTime;
            Mobile = mobile;

            if (designTime == null)
                throw new ArgumentNullException(nameof(designTime));

            if (tablet == null)
                throw new ArgumentNullException(nameof(tablet));

            if (mobile == null)
                throw new ArgumentNullException(nameof(mobile));

            if (!designTime.IsTest)
                throw new ArgumentException($"Value of {nameof(designTime)}.{nameof(designTime.IsTest)} must be true.", nameof(designTime));

            if (tablet.IsTest || mobile.IsTest)
                throw new ArgumentException($"Value of {nameof(tablet)}.{nameof(tablet.IsTest)} and {nameof(mobile)}.{nameof(mobile.IsTest)} must be false.");
        }

        public AdUnitBundle(AdUnit designTime, AdUnit adUnit) : this(designTime, adUnit, adUnit)
        {
        }

        public AdUnit DesignTime { get; }

        public AdUnit Mobile { get; }

        public AdUnit Tablet { get; }
    }
}