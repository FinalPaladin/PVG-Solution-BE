using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PVG.Domain.Models
{
    public class RQ_UserLoginModel
    {
        public string UserName { get; set; }
        public string Password { get; set; }
    }

    public class RS_UserLoginModel
    {
        public UserModel Data { get; set; } = new();
    }

    public class RQ_SearchUserModel
    {
        public bool Actived { get; set; }
        public string FullName { get; set; }
        public string UserName { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }

    public class RS_SearchUserModel
    {
        public PaginationModel<List<UserModel>> Data { get; set; } = new();
    }

    public class UserModel
    {
        public Guid? Id { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string FullName { get; set; }
        public bool Actived { get; set; }
    }

    public class RQ_GetUserModel
    {
        public string UserName { get; set; }
    }

    public class RS_GetUserModel
    {
        public UserModel Data { get; set; } = new();
    }

    public class RQ_DeleteUserModel
    {
        public string UserAction { get; set; }
        public string DeleteUser { get; set; }
    }
}
