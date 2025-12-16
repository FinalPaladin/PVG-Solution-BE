namespace PVG.Domain.Models
{
    public class ProductModel
    {
        public Guid ProductCategoryId { get; set; }
        public string Name { get; set; }
        public int LoanAmountId { get; set; }
        public int LoanTermId { get; set; }
        public string ImageUrl { get; set; }
        public bool Inactive { get; set; } = false;
    }

    public class ProductSearchRequest : PaginationModel
    {
        public string FilterKeyword { get; set; }
        public Guid? ProductCategoryId { get; set; }
    }

    public class ProductResponseModel : ProductModel
    {
        public Guid Id { get; set; }
        public Guid? CreatedBy { get; set; }
        public string? CreatedByName { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid? ModifiedBy { get; set; }
        public string? ModifiedByName { get; set; }
        public DateTime? ModifiedDate { get; set; }
    }

    public class ProductCreateRequest : ProductModel
    {
        public string UserName { get; set; }
    }

    public class ProductUpdateRequest : ProductCreateRequest
    {
        public Guid Id { get; set; }
    }

    public class ProductDetailModel
    {
        public int ProductDetailCategoryId { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
    }

    public class ProductDetailResponseModel : ProductDetailModel
    {
        public Guid Id { get; set; }
        public Guid ProductId { get; set; }
        public Guid? CreatedBy { get; set; }
        public string? CreatedByName { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid? ModifiedBy { get; set; }
        public string? ModifiedByName { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public bool IsDeleted { get; set; }
    }
}