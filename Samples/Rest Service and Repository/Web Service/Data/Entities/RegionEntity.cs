using System.Collections.Generic;

namespace Web_Service.Data.Entities
{
	public partial class RegionEntity
    {
        public RegionEntity()
        {
            Cultures = new HashSet<CultureEntity>();
        }

        public int Id { get; set; }
        public string Code { get; set; }
        public bool GeoId { get; set; }
        public int CurrencyId { get; set; }
        public string EnglishName { get; set; }
        public string NativeName { get; set; }

        public virtual ICollection<CultureEntity> Cultures { get; set; }
        public virtual CurrencyEntity Currency { get; set; }
    }
}
