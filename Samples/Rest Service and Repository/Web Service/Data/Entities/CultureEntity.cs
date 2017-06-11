using System.Collections.Generic;

namespace Web_Service.Data.Entities
{
	public partial class CultureEntity
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public int? ParentId { get; set; }
        public int Lcid { get; set; }
        public string EnglishName { get; set; }
        public string NativeName { get; set; }
        public int? RegionId { get; set; }

        public virtual CultureEntity Parent { get; set; }
        public virtual ICollection<CultureEntity> Children { get; set; }
        public virtual RegionEntity Region { get; set; }
    }
}
