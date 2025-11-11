namespace PVG.Infrastucture.Entities.BaseEntities
{
    public interface IEntityBase<T>
    {
        T Id { get; set; }
    }
}