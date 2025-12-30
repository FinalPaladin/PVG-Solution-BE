using PVG.Infrastucture.Entities.BaseEntities;

namespace PVG.Infrastucture.Entities
{
    public class ProductDetail : EntityBase<Guid>, IAudited, ISoftDelete
    {
        public Guid ProductId { get; set; }
        public int ProductDetailCategoryId { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public Guid? CreatedBy { get; set; }
        public string? CreatedByName { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid? ModifiedBy { get; set; }
        public string? ModifiedByName { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public bool IsDeleted { get; set; }
    }
}