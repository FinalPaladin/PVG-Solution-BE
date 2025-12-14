using PVG.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace PVG.Domain.Models
{
    public class NewsCategorySearchModel : PaginationModel
    {
        /// <summary>
        /// Trạng thái
        /// </summary>
        public bool? Active { get; set; }
        /// <summary>
        /// Từ khóa tìm kiếm
        /// </summary>
        public string? Keywords { get; set; }
    }

    public class CategoryModel
    {
        [Required]
        [MaxLength(255)]
        public string Name { get; set; }

        [Required]
        public bool Status { get; set; }

        [EnumDataType(typeof(NewsTypeEnum), ErrorMessage = "type_ERR_REQUIRED")]
        public NewsTypeEnum Type { get; set; }

        public int? DisplayOrder { get; set; }
        public string UserName { get; set; }
    }

    public class CategoryResponseModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public bool Status { get; set; }
        [EnumDataType(typeof(NewsTypeEnum), ErrorMessage = "type_ERR_REQUIRED")]
        public NewsTypeEnum Type { get; set; }
        public int? DisplayOrder { get; set; }
        public Guid? CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid? ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public Guid? DeletedBy { get; set; }
        public DateTime? DeletedDate { get; set; }
        public bool IsDeleted { get; set; }
    }
}