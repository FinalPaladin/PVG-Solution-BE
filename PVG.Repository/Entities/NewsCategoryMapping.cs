using PVG.Infrastucture.Entities.BaseEntities;

namespace PVG.Infrastucture.Entities
{
    public class NewsCategoryMapping : EntityBase<Guid>, IAddFullAudited
    {
        public Guid? NewsId { get; set; }
        public Guid? CategoryId { get; set; }
        public Guid? CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid? ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public Guid? DeletedBy { get; set; }
        public DateTime? DeletedDate { get; set; }
        public bool IsDeleted { get; set; }
        public string? CreatedByName { get; set; }
        public string? ModifiedByName { get; set; }
        public string DeletedByName { get; set; }
    }
}