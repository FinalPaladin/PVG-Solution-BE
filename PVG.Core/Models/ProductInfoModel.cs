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
        public string CreateUser { get; set; }
        public Guid? Id { get; set; }
        public Guid? ProductId { get; set; }
        public string Type { get; set; }
        public string Description { get; set; }
        public string Content { get; set; }
    }

    public class RQ_SearchProductInfoModel
    {
        public Guid? ProductId { get; set; }
        public string Type { get; set; }
        public string Description { get; set; }
        public string Content { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }

    public class RS_SearchProductInfoModel
    {
        public PaginationModel<List<ProductInfoModel>> Data { get; set; } = new();
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
        public string UserDelete { get; set; }
        public Guid? Id { get; set; }
    }
}
