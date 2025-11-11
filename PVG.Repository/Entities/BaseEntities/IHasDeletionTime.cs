namespace PVG.Infrastucture.Entities.BaseEntities
{
    public interface IHasDeletionTime : ISoftDelete
    {
        public DateTime? DeletedDate { get; set; }
    }
}
