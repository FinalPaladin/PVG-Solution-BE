using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PVG.Infrastucture.Entities
{
    public class ImageRequest : Sample
    {
        public string Url { get; set; }
        public Guid? RequestCode { get; set; }
        public string Content { get; set; }
    }
}
