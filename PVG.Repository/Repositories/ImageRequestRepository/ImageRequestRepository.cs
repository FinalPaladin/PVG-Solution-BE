using PVG.Infrastucture.Domain;
using PVG.Infrastucture.Entities;
using PVG.Infrastucture.Persistence;

namespace PVG.Infrastucture.Repositories.ImageRequestRepository
{
    public class ImageRequestRepository : RepositoryBase<ImageRequest, Guid>, IImageRequestRepository
    {
        protected readonly PVGDbContext dbContext;

        public ImageRequestRepository(PVGDbContext _dbContext, IUnitOfWork _unitOfWork) : base(_dbContext, _unitOfWork)
        {
            dbContext = _dbContext;
        }
    }
}