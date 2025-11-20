using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PVG.Domain.Models
{
    public class ProductCategoryModel
    {
        public Guid? Id { get; set; }
        public string Name { get; set; }
    }

    public class RQ_SaveProductCategoryModel
    {
        public Guid? CreateUserId { get; set; }
        public Guid? Id { get; set; }
        public string Name { get; set; }
    }

    public class RS_GetAllProductCategoryModel
    {
        public List<ProductCategoryModel> Data { get; set; } = new List<ProductCategoryModel>();
    }

    public class RQ_GetProductCategoryModel
    {
        public Guid? Id { get; set; }
    }

    public class RS_GetProductCategoryModel
    {
        public ProductCategoryModel Data { get; set; } = new ProductCategoryModel();
    }

    public class RQ_DeleteProductCategoryModel
    {
        public Guid? DeleteUserId { get; set; }
        public Guid? Id { get; set; }
    }
}
