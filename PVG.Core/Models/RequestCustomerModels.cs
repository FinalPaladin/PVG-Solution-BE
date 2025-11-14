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
        public string Phone { get; set; }
        public List<RequestCustomerModel> ListRequestCustomer { get; set; } = new();
    }

    public class RequestCustomerModel
    {
        public string Key { get; set; }
        public string Value { get; set; }
    }
}
