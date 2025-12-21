using PVG.Domain.Enums;
using PVG.Infrastucture.Entities.BaseEntities;
using System.ComponentModel.DataAnnotations;

namespace PVG.Infrastucture.Entities
{
    public class NewsCategory : EntityBase<Guid>, IAddFullAudited
    {
        [Required]
        [MaxLength(255)]
        public string Name { get; set; }

        [Required]
        public bool Status { get; set; }

        [Required]
        public NewsTypeEnum Type { get; set; }
        public string? Slug { get; set; }
        public int? DisplayOrder { get; set; }
        public Guid? CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid? ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public Guid? DeletedBy { get; set; }
        public DateTime? DeletedDate { get; set; }
        public bool IsDeleted { get; set; }
        public string? CreatedByName { get; set; }
        public string? ModifiedByName { get; set; }
        public string? DeletedByName { get; set; }
    }
}