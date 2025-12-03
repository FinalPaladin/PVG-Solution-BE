using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PVG.Domain.Models
{
    public class ProductInitPageModel
    {
        public List<ProductCategoryModel> Data { get; set; } = new();
    }
}
