using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PVG.Infrastucture.Entities
{
    public class RequestCustomerDetail: Sample
    {
        public Guid? RequestCode { get; set; }
        public string Key { get; set; }
        public string Value { get; set; }
    }
}
