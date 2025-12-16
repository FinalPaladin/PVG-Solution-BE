using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PVG.Domain.Models
{
    public class PermissionModel
    {
        public Guid? Id { get; set; }
        public string Name { get; set; }
    }

    public class RQ_SavePermissionModel
    {
        public Guid? CreateUserId { get; set; }
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class RQ_SearchPermissionModel
    {
        public string Name { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }

    public class RS_SearchPermissionModel
    {
        public PaginationModel<PermissionModel> Data { get; set; } = new();
    }

    public class RQ_GetPermissionModel
    {
        public Guid? Id { get; set; }
    }

    public class RS_GetPermissionModel
    {
        public PermissionModel Data { get; set; } = new();
    }

    public class RQ_DeletePermissionModel
    {
        public string UserDelete { get; set; }
        public Guid? Id { get; set; }
    }
}
