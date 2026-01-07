using PVG.Infrastucture.Entities.BaseEntities;

namespace PVG.Infrastucture.Entities
{
    public class UserPermission : EntityBase<Guid>
    {
        public Guid? UserId { get; set; }
        public int PermissionId { get; set; }
    }
}