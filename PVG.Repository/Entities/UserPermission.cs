using PVG.Infrastucture.Entities.BaseEntities;

namespace PVG.Infrastucture.Entities
{
    public class UserPermission : EntityBase<Guid>
    {
        public Guid? UserId { get; set; }
        public User User { get; set; } = null;

        public int PermissionId { get; set; }
        public Permission Permission { get; set; } = null;
    }
}