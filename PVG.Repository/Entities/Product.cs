using PVG.Infrastucture.Entities.BaseEntities;

namespace PVG.Infrastucture.Entities
{
    public class Product : EntityBase<Guid>, IAudited
    {
        public Guid ProductCategoryId { get; set; }
        public string Name { get; set; }
        public int LoanAmountId { get; set; }
        public int LoanTermId { get; set; }
        public string ImageUrl { get; set; }
        public bool Inactive { get; set; } = false;
        public Guid? CreatedBy { get; set; }
        public string? CreatedByName { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid? ModifiedBy { get; set; }
        public string? ModifiedByName { get; set; }
        public DateTime? ModifiedDate { get; set; }
    }
}