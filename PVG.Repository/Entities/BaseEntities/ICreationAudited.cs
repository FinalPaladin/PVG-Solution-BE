namespace PVG.Infrastucture.Entities.BaseEntities
{
    public interface ICreationAudited : IHasCreationTime
    {
        public Guid? CreatedBy { get; set; }
        public string CreatedByName { get; set; }
    }
}
