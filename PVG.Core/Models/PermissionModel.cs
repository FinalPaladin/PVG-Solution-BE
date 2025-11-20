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
        public Guid? Id { get; set; }
        public string Name { get; set; }
    }

    public class RS_GetAllPermissionModel
    {
        public List<PermissionModel> Data { get; set; } = new List<PermissionModel>();
    }

    public class RQ_GetPermissionModel
    {
        public Guid? Id { get; set; }
    }

    public class RS_GetPermissionModel
    {
        public PermissionModel Data { get; set; } = new PermissionModel();
    }

    public class RQ_DeletePermissionModel
    {
        public Guid? DeleteUserId { get; set; }
        public Guid? Id { get; set; }
    }
}
