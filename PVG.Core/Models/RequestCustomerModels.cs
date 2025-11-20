using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PVG.Domain.Models
{
    public class RQ_SaveRequestCustomerModel
    {
        public string Phone { get; set; }
        public Guid? ProductId { get; set; }
        public List<SaveRequestCustomerModel> Data { get; set; } = new();
    }

    public class SaveRequestCustomerModel
    {
        public string Key { get; set; }
        public string Value { get; set; }
    }

    public class RS_GetAllRequestCustomerModel
    {
        public List<GetRequestCustomerModel> Data { get; set; } = new();
    }

    public class RS_GetRequestCustomerModel
    {
        public GetRequestCustomerModel Data { get; set; } = new();
    }

    public class GetRequestCustomerModel
    {
        public Guid? RequestCode { get; set; }
        public string Phone { get; set; }
        public Guid? ProductId { get; set; }
        public List<ObjRequestCustomerModel> ListRequestCustomer { get; set; } = new();
    }

    public class ObjRequestCustomerModel
    {
        public string Key { get; set; }
        public string Value { get; set; }
    }


    public class RequestCustomerModel
    {
        public Guid? Id { get; set; }
        public Guid? RequestCode { get; set; }
        public string Phone { get; set; }
        public string Key { get; set; }
        public string Value { get; set; }
        public Guid? ProductId { get; set; }
    }

    public class RQ_DeleteRequestCustomerModel
    {
        public string UserDelete { get; set; }
        public Guid? Id { get; set; }
        public Guid? RequestCode { get; set; }
        public string Phone { get; set; }
        public Guid? ProductId { get; set; }
    }

    public class RQ_GetRequestCustomerModel
    {
        public Guid? RequestCode { get; set; }
        public string Phone { get; set; }
        public Guid? ProductId { get; set; }
    }
}
