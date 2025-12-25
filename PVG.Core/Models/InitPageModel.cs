using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PVG.Domain.Models
{
    public class ProductInitPageModel
    {
        public List<ProductCategoryModel> Data { get; set; } = new();
    }

    public class RS_DashboardInitPageModel
    {
        public int ViewHome { get; set; }
        public int ViewProduct { get; set; }
        public int ViewProducts { get; set; }
        public int ViewNews { get; set; }
        public int total { get; set; }
        public int totalProcessed { get; set; }
        public int RequestToday { get; set; }
        public int RequestTodayProcessed { get; set; }
        public int RequestYesterday { get; set; }
        public int RequestYesterdayProcessed { get; set; }
        public int RequestThisWeek { get; set; }
        public int RequestThisWeekProcessed { get; set; }
        public int RequestThisMonth { get; set; }
        public int RequestThisMonthProcessed { get; set; }
    }
}
