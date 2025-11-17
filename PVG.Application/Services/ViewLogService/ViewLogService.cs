using PVG.Infrastucture.Repositories.ProductInfoRepository;
using PVG.Infrastucture.Repositories.ViewLogRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PVG.Application.Services.ViewLogService
{
    public class ViewLogService
    {
        private readonly IViewLogRepository _viewLogRepository;

        public ViewLogService(IViewLogRepository viewLogRepository)
        {
            _viewLogRepository = viewLogRepository;
        }
    }
}
