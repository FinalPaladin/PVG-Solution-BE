namespace PVG.Infrastucture.Entities.BaseEntities
{
    public interface IModificationAudited : IHasModificationTime
    {
        public Guid? ModifiedBy { get; set; }
        public string ModifiedByName { get; set; }
    }
}
