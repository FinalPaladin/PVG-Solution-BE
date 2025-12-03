using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PVG.Domain.Models
{
    public class UserPermissionModel
    {
        public Guid? UserId { get; set; }
        public Guid? PermissionId { get; set; }
    }
}
