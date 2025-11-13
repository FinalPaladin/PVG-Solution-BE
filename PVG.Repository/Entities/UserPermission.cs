using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PVG.Infrastucture.Entities
{
    public class UserPermission : Sample
    {
        public Guid? UserId { get; set; }
        public Guid? PermissionId { get; set; }
    }
}
