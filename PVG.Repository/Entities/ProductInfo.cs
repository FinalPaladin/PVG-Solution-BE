using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PVG.Infrastucture.Entities
{
    public class ProductInfo: Sample
    {
        public Guid? ProductId { get; set; }
        public string Type { get; set; }
        public string Description { get; set; }
        public string Content { get; set; }
    }
}
