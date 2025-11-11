namespace PVG.Infrastucture.Entities.BaseEntities
{
    public interface IAudited : ICreationAudited, IHasCreationTime, IModificationAudited, IHasModificationTime
    {
    }
}
