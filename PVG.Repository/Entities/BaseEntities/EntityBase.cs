#nullable disable

namespace PVG.Infrastucture.Entities.BaseEntities
{
    public abstract class EntityBase<TKey> : IEntityBase<TKey>
    {
        public TKey Id { get; set; }
    }
}