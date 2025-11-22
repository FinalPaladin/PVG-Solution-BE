using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static PVG.Domain.Enums.NewEnum;

namespace PVG.Infrastucture.Entities
{
    public class New : Sample
    {
        public string Title { get; set; }
        public string Content { get; set; }
        public NewStatus Status { get; set; }
        public DateTime PostedDate { get; set; }
    }
}
