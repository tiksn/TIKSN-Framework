using System.Collections.Generic;

namespace Web_Service.Data.Entities
{
	public partial class CurrencyEntity
    {
        public CurrencyEntity()
        {
            Regions = new HashSet<RegionEntity>();
        }

        public int Id { get; set; }
        public string Code { get; set; }
        public int Number { get; set; }
        public string CurrencySymbol { get; set; }
        public bool IsCurrent { get; set; }
        public bool IsFund { get; set; }

        public virtual ICollection<RegionEntity> Regions { get; set; }
    }
}
