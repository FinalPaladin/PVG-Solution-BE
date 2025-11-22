using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static PVG.Domain.Enums.NewEnum;

namespace PVG.Domain.Models
{
    public class NewModel
    {
        public Guid? Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public NewStatus Status { get; set; }
        public DateTime PostedDate { get; set; }
    }

    public class RQ_SaveNewModel
    {
        public string CreateUser { get; set; }
        public Guid? Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public NewStatus Status { get; set; }
        public DateTime PostedDate { get; set; }
    }

    public class RQ_SearchNewModel
    {
        public string Title { get; set; }
        public string Content { get; set; }
        public NewStatus Status { get; set; }
        public DateTime PostedDate { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }

    public class RS_SearchNewModel
    {
        public PaginationModel<List<NewModel>> Data { get; set; } = new();
    }


    public class RQ_GetNewModel
    {
        public Guid? Id { get; set; }
    }

    public class RS_GetNewModel
    {
        public NewModel Data { get; set; } = new();
    }

    public class RQ_DeleteNewModel
    {
        public string UserDelete { get; set; }
        public Guid? Id { get; set; }
    }
}
