using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PVG.Domain.Models
{
    public class RQ_SaveRequestCustomerModel
    {
        public List<SaveRequestCustomerModel> Data { get; set; } = new();
    }

    public class SaveRequestCustomerModel
    {
        public string Phone { get; set; }
        public string Key { get; set; }
        public string Value { get; set; }
    }

    public class RS_GetRequestCustomerModel
    {
        public List<SaveRequestCustomerModel> Data { get; set; } = new();
    }

}
