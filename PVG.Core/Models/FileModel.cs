using System.ComponentModel.DataAnnotations;

namespace PVG.Domain.Models
{
    public class DTOFile
    {
        [Required(ErrorMessage = "name_ERR_REQUIRED")]
        public string FileName { get; set; }

        [Required(ErrorMessage = "path_ERR_REQUIRED")]
        public string Path { get; set; }
    }
}