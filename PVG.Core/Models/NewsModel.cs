using PVG.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace PVG.Domain.Models
{
    public class DTOSearchNews : PaginationModel
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

        public string Search { get; set; }

        [EnumDataType(typeof(NewsTypeEnum), ErrorMessage = "type_ERR_INVALID_VALUE")]
        public NewsTypeEnum? Type { get; set; }

        [EnumDataType(typeof(NewsColumn), ErrorMessage = "sortIndex_ERR_INVALID_VALUE")]
        public NewsColumn? SortIndex { get; set; }

        [EnumDataType(typeof(SortDirection), ErrorMessage = "sortDirection_ERR_INVALID_VALUE")]
        public SortDirection? SortType { get; set; }

        public DateTime? PublishFrom { get; set; }
        public DateTime? PublishTo { get; set; }
    }
}