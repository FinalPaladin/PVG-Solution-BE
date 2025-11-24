namespace PVG.Infrastucture.Entities
{
    public class UserPermission
    {
        public Guid? UserId { get; set; }
        public User User { get; set; } = default;

        public int PermissionId { get; set; }
        public Permission Permission { get; set; } = default;
    }
}