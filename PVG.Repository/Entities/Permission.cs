using PVG.Infrastucture.Entities.BaseEntities;

namespace PVG.Infrastucture.Entities
{
    public class Permission : EntityBase<int>
    {
        public string Code { get; set; }
        public ICollection<UserPermission> UserPermissions { get; set; } = new List<UserPermission>();
    }
}