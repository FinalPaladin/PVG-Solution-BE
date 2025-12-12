using PVG.Domain.Enums;
using PVG.Infrastucture.Entities.BaseEntities;

namespace PVG.Infrastucture.Entities
{
    public class MData : EntityBase<int>, IAudited, ISoftDelete
    {
        public MDataEnum_Group Group { get; set; }
        public string? GroupName { get; set; }
        public int SortId { get; set; }
        public string Key { get; set; }
        public string Value { get; set; }
        public string? Value_ENG { get; set; }
        public bool Inactive { get; set; } = false;
        public Guid? CreatedBy { get; set; }
        public string? CreatedByName { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid? ModifiedBy { get; set; }
        public string? ModifiedByName { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public bool IsDeleted { get; set; }
    }
}