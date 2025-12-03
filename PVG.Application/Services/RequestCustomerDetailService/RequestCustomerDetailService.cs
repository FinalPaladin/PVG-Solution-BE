using AutoMapper;
using PVG.Application.Services.UserService;
using PVG.Infrastucture.Repositories.ProductRepository;
using PVG.Infrastucture.Repositories.UserRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PVG.Application.Services.RequestCustomerDetailService
{
    public class RequestCustomerDetailService: IRequestCustomerDetailService
    {
        private readonly IMapper _mapper;
        private readonly IUserRepository _userRepository;
        private readonly IUserService _userService;

        public RequestCustomerDetailService(
            IMapper mapper,
            IUserRepository userRepository,
            IUserService userService)
        {
            _mapper = mapper;
            _userRepository = userRepository;
            _userService = userService;
        }

    }
}
