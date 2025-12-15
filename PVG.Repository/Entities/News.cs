using PVG.Domain.Enums;
using PVG.Infrastucture.Entities.BaseEntities;
using System.ComponentModel.DataAnnotations;

namespace PVG.Infrastucture.Entities
{
    public class News : EntityBase<Guid>, IAddFullAudited
    {
        public int? Code { get; set; }

        [Required]
        [MaxLength(2000)]
        public string Title { get; set; }

        public string Description { get; set; }

        public string Content { get; set; }

        public string Slug { get; set; }

        [Required]
        public bool Active { get; set; }

        [Required]
        public NewsTypeEnum Type { get; set; }

        [Required]
        public bool NeedApproved { get; set; }

        public DateTime? ApprovedDate { get; set; }

        public Guid? ApprovedBy { get; set; }
        public bool? IsApproved { get; set; }

        [Required]
        public DateTime PublishDate { get; set; }

        public DateTime? ExpireDate { get; set; }

        [MaxLength(1000)]
        public string ImageLink { get; set; }

        public string ImageName { get; set; }

        [MaxLength(1000)]
        public string Thumbnail { get; set; }

        public string ThumbnailName { get; set; }

        public int? DisplayOrder { get; set; }
        public Guid? CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid? ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public Guid? DeletedBy { get; set; }
        public DateTime? DeletedDate { get; set; }
        public bool IsDeleted { get; set; }
        public string? CreatedByName { get ; set ; }
        public string? ModifiedByName { get ; set ; }
        public string? DeletedByName { get ; set ; }
    }
}