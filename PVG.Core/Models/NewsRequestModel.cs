using System.ComponentModel.DataAnnotations;

namespace PVG.Domain.Models
{
    public class DTONewsRequest
    {
        public DTONewsRequest()
        {
            Files = new List<DTOFile>();
        }

        [Required(ErrorMessage = "title_ERR_REQUIRED")]
        [MaxLength(100, ErrorMessage = "title_ERR_MAX_LENGTH_100")]
        public string Title { get; set; }

        [Required(ErrorMessage = "active_ERR_REQUIRED")]
        public bool Active { get; set; }

        [Required(ErrorMessage = "content_ERR_REQUIRED")]
        [MaxLength(50000, ErrorMessage = "content_ERR_MAX_LENGTH_50000")]
        public string Content { get; set; }

        public string Description { get; set; }
        public DateTime? PublishDate { get; set; }
        public DateTime? ExpireDate { get; set; }

        [Required(ErrorMessage = "needApprove_ERR_REQUIRED")]
        public bool NeedApprove { get; set; }

        public DTOFile ThumbnailFile { get; set; }
        public List<DTOFile> Files { get; set; }

        [Required(ErrorMessage = "isNotify_ERR_REQUIRED")]
        public bool? IsNotify { get; set; } = false;

        public int? DisplayOrder { get; set; }
    }
}