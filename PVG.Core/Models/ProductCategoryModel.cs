namespace PVG.Domain.Models
{
    public class ProductCategoryModel
    {
        public Guid? Id { get; set; }
        public string Name { get; set; }
        public List<ProductModel> Products { get; set; } = new();
    }

    public class RQ_SaveProductCategoryModel
    {
        public string Name { get; set; }
        public bool Inactive { get; set; } = false;
        public string CreatedBy { get; set; }
    }

    public class RQ_SearchProductCategoryModel
    {
        public string? keyword { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }

    public class RQ_GetProductCategoryModel
    {
        public Guid? Id { get; set; }
    }

    public class RQ_UpdateProductCategoryModel : RQ_SaveProductCategoryModel
    {
        public Guid? Id { get; set; }
    }
}