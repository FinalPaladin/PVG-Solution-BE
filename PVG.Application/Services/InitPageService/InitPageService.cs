using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PVG.Core.BaseModels;
using PVG.Domain.Models;
using PVG.Infrastucture.Entities;
using PVG.Infrastucture.Repositories.ConfigurationRepository;
using PVG.Infrastucture.Repositories.PermissionRepository;
using PVG.Infrastucture.Repositories.ProductCategoryRepository;
using PVG.Infrastucture.Repositories.ProductRepository;
using PVG.Infrastucture.Repositories.RequestCustomerRepository;
using PVG.Infrastucture.Repositories.UserPermissionRepository;
using PVG.Infrastucture.Repositories.UserRepository;
using PVG.Infrastucture.Repositories.ViewLogRepository;

namespace PVG.Application.Services.InitPageService
{
    public class InitPageService : IInitPageService
    {
        private readonly IMapper _mapper;
        private readonly IProductRepository _productRepository;
        private readonly IProductCategoryRepository _productCategoryRepository;
        private readonly IUserRepository _userRepository;
        private readonly IPermissionRepository _permissionRepository;
        private readonly IUserPermissionRepository _userPermissionRepository;
        private readonly IConfigurationRepository _configurationRepository;
        private readonly IPasswordHasher<User> _passwordHasher;
        private readonly IViewLogRepository _viewLogRepository;
        private readonly IRequestCustomerRepository _requestCustomerRepository;

        public InitPageService(IProductRepository productRepository,
            IProductCategoryRepository productCategoryRepository,
            IMapper mapper,
            IUserRepository userRepository,
            IPermissionRepository permissionRepository,
            IUserPermissionRepository userPermissionRepository,
            IConfigurationRepository configurationRepository,
            IPasswordHasher<User> passwordHasher,
            IViewLogRepository viewLogRepository,
            IRequestCustomerRepository requestCustomerRepository)
        {
            _mapper = mapper;
            _productRepository = productRepository;
            _productCategoryRepository = productCategoryRepository;
            _userRepository = userRepository;
            _permissionRepository = permissionRepository;
            _userPermissionRepository = userPermissionRepository;
            _configurationRepository = configurationRepository;
            _passwordHasher = passwordHasher;
            _viewLogRepository = viewLogRepository;
            _requestCustomerRepository = requestCustomerRepository;
        }

        public async Task<BaseResponse<ProductInitPageModel>> Product()
        {
            try
            {
                var productCategoriesEntity = await _productCategoryRepository.FindByCondition(x => !x.Inactive).ToListAsync();

                if (productCategoriesEntity == null || productCategoriesEntity.Count == 0)
                {
                    return new BaseResponse<ProductInitPageModel>()
                    {
                        IsSuccess = false,
                        StatusCode = StatusCodes.Status400BadRequest,
                        Message = "Dữ liệu chưa được định nghĩa"
                    };
                }

                var productcategories = _mapper.Map<List<ProductCategoryModel>>(productCategoriesEntity);

                var productsEntity = await _productRepository.FindByCondition(x => !x.Inactive).ToListAsync();

                if (productCategoriesEntity != null && productCategoriesEntity.Count > 0)
                {
                    var products = _mapper.Map<List<ProductModel>>(productsEntity);
                    foreach (var pce in productcategories)
                    {
                        var productsFilter = products.Where(x => x.ProductCategoryId == pce.Id).ToList();

                        if (productsFilter != null && productsFilter.Count > 0)
                            pce.Products = productsFilter;
                        else
                            pce.Products = new();
                    }
                }

                return new BaseResponse<ProductInitPageModel>()
                {
                    IsSuccess = true,
                    StatusCode = StatusCodes.Status404NotFound,
                    Message = "Lấy dữ liệu thành công",
                    Result = new()
                    {
                        Data = productcategories
                    }
                };
            }
            catch (Exception ex)
            {
                return new BaseResponse<ProductInitPageModel>()
                {
                    IsSuccess = false,
                    StatusCode = StatusCodes.Status404NotFound,
                    Message = ex.Message,
                };
            }
        }

        public async Task<BaseResponse> System()
        {
            try
            {
                var usersExist = await _userRepository.FindAll().ToListAsync();

                if (usersExist != null && usersExist.Count > 0)
                    return new BaseResponse()
                    {
                        IsSuccess = false,
                        StatusCode = StatusCodes.Status404NotFound,
                        Message = "Hàm khởi tạo đã chạy",
                    };

                var initUsers = new List<User>
                {
                    new User()
                    {
                        Id = Guid.NewGuid(),
                        Actived = true,
                        FullName = "System Admin",
                        UserName = "systemadmin",
                        Password = "pw123qwe"
                    },
                    new User()
                    {
                        Id = Guid.NewGuid(),
                        Actived = true,
                        FullName = "Marketing Admin",
                        UserName = "mktadmin",
                        Password = "p@ssw0rd8888"
                    },
                    new User()
                    {
                        Id = Guid.NewGuid(),
                        Actived = true,
                        FullName = "Sales Admin",
                        UserName = "salesadmin",
                        Password = "p@ssw0rd9999"
                    },
                    new User()
                    {
                        Id = Guid.NewGuid(),
                        Actived = true,
                        FullName = "IT Admin",
                        UserName = "itadmin",
                        Password = "itpgv"
                    }
                };

                foreach (var user in initUsers)
                {
                    var passhash = _passwordHasher.HashPassword(user, user.Password);
                    user.Password = passhash;
                }

                await _userRepository.CreateListAsync(initUsers);
                await _userRepository.SaveChangesAsync();

                var initPermissions = new List<Permission>
                {
                    new Permission()
                    {
                        Id = 1,
                        Code = "SYS_AD",
                    },
                    new Permission()
                    {
                        Id = 2,
                        Code = "MKT_AD",
                    },
                    new Permission()
                    {
                        Id = 3,
                        Code = "SALES_AD",
                    }
                };
                await _permissionRepository.CreateListAsync(initPermissions);
                await _permissionRepository.SaveChangesAsync();

                var users = await _userRepository.FindAll().ToListAsync();

                if (users == null || users.Count == 0)
                    return new BaseResponse()
                    {
                        IsSuccess = false,
                        StatusCode = StatusCodes.Status404NotFound,
                        Message = "",
                    };
                var initUserPermissions = new List<UserPermission>
                {
                    new UserPermission()
                    {
                        Id = Guid.NewGuid(),
                        UserId = users.Find(x => x.UserName == "itadmin")?.Id,
                        PermissionId = 1,
                    },
                    new UserPermission()
                    {
                        Id = Guid.NewGuid(),
                        UserId = users.Find(x => x.UserName == "systemadmin")?.Id,
                        PermissionId = 1,
                    },
                    new UserPermission()
                    {
                        Id = Guid.NewGuid(),
                        UserId = users.Find(x => x.UserName == "mktadmin")?.Id,
                        PermissionId = 2,
                    },
                    new UserPermission()
                    {
                        Id = Guid.NewGuid(),
                        UserId = users.Find(x => x.UserName == "salesadmin")?.Id,
                        PermissionId = 3,
                    }
                };
                await _userPermissionRepository.CreateListAsync(initUserPermissions);
                await _userPermissionRepository.SaveChangesAsync();

                var sysADId = initUsers.Find(x => x.UserName == "systemadmin")?.Id;
                var initConfigs = new List<Configuration>
                {
                    new Configuration()
                    {
                        Id = Guid.NewGuid(),
                        Key = "EmailFromName",
                        Value = "PVG Service",
                        CreatedBy = sysADId,
                        CreatedDate = DateTime.Now,
                        CreatedByName = "System Admin",
                        DeletedBy = null,
                        DeletedByName = "",
                        DeletedDate = DateTime.Now,
                        IsDeleted = false,
                        ModifiedBy = null,
                        ModifiedByName = "",
                        ModifiedDate = DateTime.Now,
                        IsImage = false,
                    },
                    new Configuration()
                    {
                        Id = Guid.NewGuid(),
                        Key = "EmailSend",
                        Value = "customer.form.request@gmail.com",
                        CreatedBy = sysADId,
                        CreatedDate = DateTime.Now,
                        CreatedByName = "System Admin",
                        DeletedBy = null,
                        DeletedByName = "",
                        DeletedDate = DateTime.Now,
                        IsDeleted = false,
                        ModifiedBy = null,
                        ModifiedByName = "",
                        ModifiedDate = DateTime.Now,
                        IsImage = false,
                    },
                    new Configuration()
                    {
                        Id = Guid.NewGuid(),
                        Key = "EmailSendPassword",
                        Value = "zqls zmir wxxx yvnw",
                        CreatedBy = sysADId,
                        CreatedDate = DateTime.Now,
                        CreatedByName = "System Admin",
                        DeletedBy = null,
                        DeletedByName = "",
                        DeletedDate = DateTime.Now,
                        IsDeleted = false,
                        ModifiedBy = null,
                        ModifiedByName = "",
                        ModifiedDate = DateTime.Now,
                        IsImage = false,
                    },
                    new Configuration()
                    {
                        Id = Guid.NewGuid(),
                        Key = "EmailReceive",
                        Value = "customer.service.csone@gmail.com",
                        CreatedBy = sysADId,
                        CreatedDate = DateTime.Now,
                        CreatedByName = "System Admin",
                        DeletedBy = null,
                        DeletedByName = "",
                        DeletedDate = DateTime.Now,
                        IsDeleted = false,
                        ModifiedBy = null,
                        ModifiedByName = "",
                        ModifiedDate = DateTime.Now,
                        IsImage = false,
                    },
                    new Configuration()
                    {
                        Id = Guid.NewGuid(),
                        Key = "EmailSmtpHost",
                        Value = "smtp.gmail.com",
                        CreatedBy = sysADId,
                        CreatedDate = DateTime.Now,
                        CreatedByName = "System Admin",
                        DeletedBy = null,
                        DeletedByName = "",
                        DeletedDate = DateTime.Now,
                        IsDeleted = false,
                        ModifiedBy = null,
                        ModifiedByName = "",
                        ModifiedDate = DateTime.Now,
                        IsImage = false,
                    },
                    new Configuration()
                    {
                        Id = Guid.NewGuid(),
                        Key = "EmailPort",
                        Value = "587",
                        CreatedBy = sysADId,
                        CreatedDate = DateTime.Now,
                        CreatedByName = "System Admin",
                        DeletedBy = null,
                        DeletedByName = "",
                        DeletedDate = DateTime.Now,
                        IsDeleted = false,
                        ModifiedBy = null,
                        ModifiedByName = "",
                        ModifiedDate = DateTime.Now,
                        IsImage = false,
                    },
                    new Configuration()
                    {
                        Id = Guid.NewGuid(),
                        Key = "SDTSales",
                        Value = "",
                        CreatedBy = sysADId,
                        CreatedDate = DateTime.Now,
                        CreatedByName = "System Admin",
                        DeletedBy = null,
                        DeletedByName = "",
                        DeletedDate = DateTime.Now,
                        IsDeleted = false,
                        ModifiedBy = null,
                        ModifiedByName = "",
                        ModifiedDate = DateTime.Now,
                        IsImage = false,
                    },
                    new Configuration()
                    {
                        Id = Guid.NewGuid(),
                        Key = "ImgLogo",
                        Value = "",
                        CreatedBy = sysADId,
                        CreatedDate = DateTime.Now,
                        CreatedByName = "System Admin",
                        DeletedBy = null,
                        DeletedByName = "",
                        DeletedDate = DateTime.Now,
                        IsDeleted = false,
                        ModifiedBy = null,
                        ModifiedByName = "",
                        ModifiedDate = DateTime.Now,
                        IsImage = true,
                    },
                    new Configuration()
                    {
                        Id = Guid.NewGuid(),
                        Key = "ImgHome",
                        Value = "",
                        CreatedBy = sysADId,
                        CreatedDate = DateTime.Now,
                        CreatedByName = "System Admin",
                        DeletedBy = null,
                        DeletedByName = "",
                        DeletedDate = DateTime.Now,
                        IsDeleted = false,
                        ModifiedBy = null,
                        ModifiedByName = "",
                        ModifiedDate = DateTime.Now,
                        IsImage = true,
                    },
                    new Configuration()
                    {
                        Id = Guid.NewGuid(),
                        Key = "ImgBackground",
                        Value = "",
                        CreatedBy = sysADId,
                        CreatedDate = DateTime.Now,
                        CreatedByName = "System Admin",
                        DeletedBy = null,
                        DeletedByName = "",
                        DeletedDate = DateTime.Now,
                        IsDeleted = false,
                        ModifiedBy = null,
                        ModifiedByName = "",
                        ModifiedDate = DateTime.Now,
                        IsImage = true,
                    },
                };
                await _configurationRepository.CreateListAsync(initConfigs);
                await _configurationRepository.SaveChangesAsync();

                return new BaseResponse()
                {
                    IsSuccess = true,
                    StatusCode = StatusCodes.Status200OK,
                };
            }
            catch (Exception ex)
            {
                return new BaseResponse()
                {
                    IsSuccess = false,
                    StatusCode = StatusCodes.Status404NotFound,
                    Message = ex.Message,
                };
            }
        }
    
        public async Task<BaseResponse<RS_DashboardInitPageModel>> Dashboard()
        {
            try
            {
                var viewlogRepo = await _viewLogRepository.FindByCondition(x => !x.IsDeleted).ToListAsync();

                int viewhome = 0, viewproduct = 0, viewproducts = 0, viewnews = 0;

                if(viewlogRepo != null && viewlogRepo.Count > 0)
                {
                    viewhome = viewlogRepo.Where(x => x.Screen == Domain.Enums.ViewLogEnum.ScreenView.Home).ToList().Count;
                    viewproduct = viewlogRepo.Where(x => x.Screen == Domain.Enums.ViewLogEnum.ScreenView.Product).ToList().Count;
                    viewproducts = viewlogRepo.Where(x => x.Screen == Domain.Enums.ViewLogEnum.ScreenView.Products).ToList().Count;
                    viewnews = viewlogRepo.Where(x => x.Screen == Domain.Enums.ViewLogEnum.ScreenView.News).ToList().Count;
                }

                var request = await _requestCustomerRepository.FindByCondition(x => !x.IsDeleted).ToListAsync();

                int total = 0, totalProcessed = 0, rqtoday = 0, todayProcessed = 0, yesterday = 0, yesterdayProcessed = 0, thisweek = 0, thisweekProcessed = 0, thismonth = 0, thismonthProcessed = 0;

                if(request !=null && request.Count > 0)
                {
                    total = request.Count;
                    totalProcessed = request.Where(x => x.IsProcessed).ToList().Count;

                    DateTime today = DateTime.Now;
                    var startOfMonth = new DateTime(today.Year, today.Month, 1);
                    var startOfNextMonth = startOfMonth.AddMonths(1);

                    var listmonth = request.Where(x => x.CreatedDate >= startOfMonth && x.CreatedDate < startOfNextMonth).ToList();

                    if(listmonth !=null && listmonth.Count > 0)
                    {
                        thismonth = listmonth.Count;
                        thismonthProcessed = listmonth.Where(x => x.IsProcessed).ToList().Count;

                        int diff = (7 + (today.DayOfWeek - DayOfWeek.Monday)) % 7;
                        var startOfWeek = today.AddDays(-diff);
                        var startOfNextWeek = startOfWeek.AddDays(7);

                        var listweek = listmonth.Where(x => x.CreatedDate >= startOfWeek && x.CreatedDate < startOfNextWeek).ToList();

                        if (listweek != null && listweek.Count > 0)
                        {
                            thisweek = listweek.Count;
                            thisweekProcessed = listweek.Where(x => x.IsProcessed).ToList().Count;

                            var startOfYesterday = today.AddDays(-1);
                            var endOfToday = new DateTime(today.Year, today.Month, today.Day, 23, 59, 59);

                            var listyesterday = listweek.Where(x => x.CreatedDate >= startOfYesterday && x.CreatedDate < endOfToday).ToList();

                            if (listyesterday != null && listyesterday.Count > 0)
                            {
                                yesterday = listyesterday.Count;
                                yesterdayProcessed = listyesterday.Where(x => x.IsProcessed).ToList().Count;

                                var startOfToday = new DateTime(today.Year, today.Month, today.Day, 0, 0, 0);
                                var listtoday = listweek.Where(x => x.CreatedDate >= startOfToday && x.CreatedDate < endOfToday).ToList();
                                if (listtoday != null && listtoday.Count > 0)
                                {
                                    rqtoday = listtoday.Count;
                                    todayProcessed = listtoday.Where(x => x.IsProcessed).ToList().Count;
                                }
                            }
                        }
                    }
                }

                return new BaseResponse<RS_DashboardInitPageModel>()
                {
                    IsSuccess = true,
                    StatusCode = StatusCodes.Status200OK,
                    Message = "Lấy dữ liệu thành công",
                    Result = new()
                    {
                        ViewHome = viewhome,
                        ViewNews = viewnews,
                        ViewProduct = viewproduct,
                        ViewProducts = viewproducts,
                        total = total,
                        totalProcessed = totalProcessed,
                        RequestToday = rqtoday,
                        RequestTodayProcessed = todayProcessed,
                        RequestThisMonth = thismonth,
                        RequestThisWeek = thisweek,
                        RequestYesterday = yesterday,
                        RequestThisMonthProcessed = thismonthProcessed,
                        RequestThisWeekProcessed = thisweekProcessed,
                        RequestYesterdayProcessed = yesterdayProcessed,
                    }
                };
            }
            catch (Exception ex)
            {
                return new BaseResponse<RS_DashboardInitPageModel>()
                {
                    IsSuccess = false,
                    StatusCode = StatusCodes.Status404NotFound,
                    Message = ex.Message,
                };
            }
        }
    }
}