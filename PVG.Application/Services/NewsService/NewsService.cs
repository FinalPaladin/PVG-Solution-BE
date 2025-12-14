using AutoMapper;
using Microsoft.Extensions.Options;
using PVG.Domain.Settings;

namespace PVG.Application.Services.NewsService
{
    public class NewsService : BaseService, INewsService
    {
        public NewsService(IOptions<AppSettings> settings, IMapper mapper) : base(settings, mapper)
        {
        }
    }
}