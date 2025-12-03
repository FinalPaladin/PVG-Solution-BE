using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PVG.Application.Services.PermissionService;
using PVG.Application.Services.UserPermissionService;
using PVG.Application.Services.UserService;
using PVG.Core.BaseModels;
using PVG.Domain.Models;
using PVG.Infrastucture.Entities;
using PVG.Infrastucture.Repositories.ConfigurationRepository;
using PVG.Infrastucture.Repositories.PermissionRepository;
using PVG.Infrastucture.Repositories.ProductCategoryRepository;
using PVG.Infrastucture.Repositories.ProductRepository;
using PVG.Infrastucture.Repositories.UserPermissionRepository;
using PVG.Infrastucture.Repositories.UserRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace PVG.Application.Services.InitPageService
{
    public class InitPageService: IInitPageService
    {
        private readonly IMapper _mapper;
        private readonly IProductRepository _productRepository;
        private readonly IProductCategoryRepository _productCategoryRepository;
        private readonly IUserRepository _userRepository;
        private readonly IPermissionRepository _permissionRepository;
        private readonly IUserPermissionRepository _userPermissionRepository;
        private readonly IConfigurationRepository _configurationRepository;
        private readonly IPasswordHasher<User> _passwordHasher;

        public InitPageService(IProductRepository productRepository,
            IProductCategoryRepository productCategoryRepository,
            IMapper mapper,
            IUserRepository userRepository,
            IPermissionRepository permissionRepository,
            IUserPermissionRepository userPermissionRepository,
            IConfigurationRepository configurationRepository,
            IPasswordHasher<User> passwordHasher)
        {
            _mapper = mapper;
            _productRepository = productRepository;
            _productCategoryRepository = productCategoryRepository;
            _userRepository = userRepository;
            _permissionRepository = permissionRepository;
            _userPermissionRepository = userPermissionRepository;
            _configurationRepository = configurationRepository;
            _passwordHasher = passwordHasher;
        }

        public async Task<BaseResponse<ProductInitPageModel>> Product()
        {
            try
            {
                var productCategoriesEntity = await _productCategoryRepository.FindByCondition(x => !x.IsDeleted).ToListAsync();

                if(productCategoriesEntity == null || productCategoriesEntity.Count == 0)
                {
                    return new BaseResponse<ProductInitPageModel>()
                    {
                        IsSuccess = false,
                        StatusCode = StatusCodes.Status400BadRequest,
                        Message = "Dữ liệu chưa được định nghĩa"
                    };
                }

                var productcategories = _mapper.Map<List<ProductCategoryModel>>(productCategoriesEntity);

                var productsEntity = await _productRepository.FindByCondition(x => !x.IsDeleted).ToListAsync();

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
                        Message = "",
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

    }
}
