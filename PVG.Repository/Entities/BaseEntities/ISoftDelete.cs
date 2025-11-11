namespace PVG.Infrastucture.Entities.BaseEntities
{
    public interface ISoftDelete
    {
        public bool IsDeleted { get; set; }
    }
}
