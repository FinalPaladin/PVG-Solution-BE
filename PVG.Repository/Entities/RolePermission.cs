using PVG.Infrastucture.Entities.BaseEntities;

namespace PVG.Infrastucture.Entities
{
    public class RolePermission : EntityBase<int>
    {
        public int RoleId { get; set; }
        public int PermissionId { get; set; }

        public bool CanView { get; set; } = false;
        public bool CanCreate { get; set; } = false;
        public bool CanEdit { get; set; } = false;
        public bool CanDelete { get; set; } = false;
        public bool CanReport { get; set; } = false;
        public bool CanApprove { get; set; } = false;
    }
}