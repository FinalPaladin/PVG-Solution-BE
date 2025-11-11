namespace PVG.Infrastucture.Entities.BaseEntities
{
    public interface IAddFullAudited : IAudited, ICreationAudited, IHasCreationTime, IModificationAudited, IHasModificationTime, IDeletionAudited, IHasDeletionTime, ISoftDelete
    {
    }
}
