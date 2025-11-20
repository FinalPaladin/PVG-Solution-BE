using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PVG.Domain.Models
{
    public class ProductModel
    {
        public Guid? Id { get; set; }
        public Guid? ProductCategoryId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Image { get; set; }
    }

    public class RQ_SaveProductModel
    {
        public Guid? CreateUserId { get; set; }
        public Guid? Id { get; set; }
        public Guid? ProductCategoryId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Image { get; set; }
    }

    public class RS_GetAllProductModel
    {
        public List<ProductModel> Data { get; set; } = new List<ProductModel>();
    }

    public class RQ_GetProductModel
    {
        public Guid? Id { get; set; }
    }

    public class RS_GetProductModel
    {
        public ProductModel Data { get; set; } = new ProductModel();
    }

    public class RQ_DeleteProductModel
    {
        public Guid? DeleteUserId { get; set; }
        public Guid? Id { get; set; }
    }
}
