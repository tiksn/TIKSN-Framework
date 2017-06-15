using System;
using TIKSN.Data;

namespace Memcache.Models
{
    public class ProductModel : IEntity<Guid>
    {
        public Guid ID { get; set; }

        public string Name { get; set; }
    }
}
