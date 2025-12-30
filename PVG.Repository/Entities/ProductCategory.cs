using PVG.Infrastucture.Entities.BaseEntities;

namespace PVG.Infrastucture.Entities
{
    public class ProductCategory : EntityBase<Guid>, IAudited
    {
        public string Name { get; set; }
        public bool Inactive { get; set; } = false;
        public Guid? CreatedBy { get; set; }
        public string CreatedByName { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid? ModifiedBy { get; set; }
        public string ModifiedByName { get; set; }
        public DateTime? ModifiedDate { get; set; }
    }
}