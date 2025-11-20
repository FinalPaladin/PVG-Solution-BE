using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PVG.Domain.Models
{
    public class ProductInfoModel
    {
        public Guid? Id { get; set; }
        public Guid? ProductId { get; set; }
        public string Type { get; set; }
        public string Description { get; set; }
        public string Content { get; set; }
    }

    public class RQ_SaveProductInfoModel
    {
        public Guid? CreateUserId { get; set; }
        public Guid? Id { get; set; }
        public Guid? ProductId { get; set; }
        public string Type { get; set; }
        public string Description { get; set; }
        public string Content { get; set; }
    }

    public class RS_GetAllProductInfoModel
    {
        public List<ProductInfoModel> Data { get; set; } = new List<ProductInfoModel>();
    }

    public class RQ_GetProductInfoModel
    {
        public Guid? Id { get; set; }
    }

    public class RS_GetProductInfoModel
    {
        public ProductInfoModel Data { get; set; } = new ProductInfoModel();
    }

    public class RQ_DeleteProductInfoModel
    {
        public Guid? DeleteUserId { get; set; }
        public Guid? Id { get; set; }
    }
}
