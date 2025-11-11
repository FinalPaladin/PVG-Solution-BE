namespace PVG.Infrastucture.Entities.BaseEntities
{
    public interface IHasModificationTime
    {
        public DateTime? ModifiedDate { get; set; }
    }
}
