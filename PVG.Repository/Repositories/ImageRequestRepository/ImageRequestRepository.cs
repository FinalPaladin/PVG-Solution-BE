using PVG.Infrastucture.Domain;
using PVG.Infrastucture.Entities;

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