using PVG.Infrastucture.Entities.BaseEntities;

namespace PVG.Infrastucture.Entities
{
    public class AuthToken : EntityBase<Guid>
    {
        public string Token { get; set; } = default!;
        public Guid UserId { get; set; }
        public User User { get; set; } = default!;
        public DateTime ExpiresAt { get; set; }
        public bool IsRevoked { get; set; }
    }
}