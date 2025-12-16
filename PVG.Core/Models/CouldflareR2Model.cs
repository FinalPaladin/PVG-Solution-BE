using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PVG.Domain.Models
{
    public class RQ_CloudflareUploadListImageModel
    {
        public List<ImageRequestCustomerModel> DataImage { get; set; } = new();
    }

    public class RS_CloudflareUploadListImageModel
    {
        public List<CloudflareUploadModel> Data { get; set; } = new();
    }

    public class CloudflareUploadModel
    {
        public string PublicUrl { get; set; }
        public string Key { get; set; }
    }
}
