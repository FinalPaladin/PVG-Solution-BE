using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PVG.Domain.Models
{
    public class RQ_SaveViewLogModel
    {
        public string IP { get; set; }
    }

    public class RQ_ViewLogModel 
    {
        public string IP { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
    }

    public class RS_ViewLogModel
    {
        public int View { get; set; }
    }
}
