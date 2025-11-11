namespace PVG.Infrastucture.Entities.BaseEntities
{
    public interface IDeletionAudited : IHasDeletionTime, ISoftDelete
    {
        public Guid? DeletedBy { get; set; }
        public string DeletedByName { get; set; }
    }
}
