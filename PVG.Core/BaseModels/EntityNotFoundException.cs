namespace PVG.Core.BaseModels
{
    public class EntityNotFoundException : ApplicationException
    {
        public EntityNotFoundException() : base("Entity was not found.")
        {
        }
    }
}