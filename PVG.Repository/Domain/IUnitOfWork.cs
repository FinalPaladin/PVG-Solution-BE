namespace PVG.Infrastucture.Domain
{
    public interface IUnitOfWork : IDisposable
    {
        Task<int> CommitAsync();
    }
}