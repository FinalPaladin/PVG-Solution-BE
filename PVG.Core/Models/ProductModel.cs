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
        public string CreateUser { get; set; }
        public Guid? Id { get; set; }
        public Guid? ProductCategoryId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Image { get; set; }
    }

    public class RQ_SearchProductModel
    {
        public Guid? ProductCategoryId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }

    public class RS_SearchProductModel
    {
        public PaginationModel<List<ProductModel>> Data { get; set; } = new();
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
        public string UserDelete { get; set; }
        public Guid? Id { get; set; }
    }
}