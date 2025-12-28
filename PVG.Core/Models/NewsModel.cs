using PVG.Domain.BaseModels;
using PVG.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace PVG.Domain.Models
{
    public class DTOSearchNews : RequestPagingModel
    {
        public DTOSearchNews()
        {
            SortIndex = null;
            SortType = SortDirection.Desc;
        }

        public Guid? CategoryId { get; set; }

        public bool? Active { get; set; }

        public bool? HasDisplayOrder { get; set; }

        public bool? ForApp { get; set; }

        public string? Search { get; set; }

        [EnumDataType(typeof(NewsTypeEnum), ErrorMessage = "type_ERR_INVALID_VALUE")]
        public NewsTypeEnum? Type { get; set; }

        [EnumDataType(typeof(NewsColumn), ErrorMessage = "sortIndex_ERR_INVALID_VALUE")]
        public NewsColumn? SortIndex { get; set; }

        [EnumDataType(typeof(SortDirection), ErrorMessage = "sortDirection_ERR_INVALID_VALUE")]
        public SortDirection? SortType { get; set; }

        public DateTime? PublishFrom { get; set; }
        public DateTime? PublishTo { get; set; }

        public DateTime? CreatedDateFrom { get; set; }
        public DateTime? CreatedDateTo { get; set; }
    }

    public class NewsResponseModel
    {
        public Guid Id { get; set; }
        public int? Code { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Content { get; set; }
        public string Slug { get; set; }
        public bool Active { get; set; }
        [EnumDataType(typeof(NewsTypeEnum), ErrorMessage = "type_ERR_REQUIRED")]
        public NewsTypeEnum Type { get; set; }
        public bool NeedApproved { get; set; }
        public DateTime? ApprovedDate { get; set; }
        public Guid? ApprovedBy { get; set; }
        public bool? IsApproved { get; set; }
        public DateTime PublishDate { get; set; }
        public DateTime? ExpireDate { get; set; }
        public string ImageLink { get; set; }
        public string ImageName { get; set; }
        public string Thumbnail { get; set; }
        public string ThumbnailName { get; set; }
        public int? DisplayOrder { get; set; }
        public Guid? CategoryId { get; set; }
        public string? CategoryName { get; set; }
        public string? CategorySlug { get; set; }
        public Guid? CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string? CreatedByName { get; set; }
        public Guid? ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
    }

    public class DTONewsOrderRequest
    {
        public DTONewsOrderRequest()
        {
            Details = new List<DTONewsOrderDetailRequest>();
        }

        [EnumDataType(typeof(NewsTypeEnum), ErrorMessage = "type_ERR_INVALID_VALUE")]
        public NewsTypeEnum? Type { get; set; }

        public List<DTONewsOrderDetailRequest> Details { get; set; }
    }

    public class DTONewsOrderDetailRequest
    {
        [Required(ErrorMessage = "code_ERR_REQUIRED")]
        public int Code { get; set; }

        [Required(ErrorMessage = "displayOrder_ERR_REQUIRED")]
        public int DisplayOrder { get; set; }
    }
}