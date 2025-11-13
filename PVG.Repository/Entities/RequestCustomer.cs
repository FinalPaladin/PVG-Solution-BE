using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PVG.Infrastucture.Entities
{
    public class RequestCustomer: Sample
    {
        public Guid? Id { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Key { get; set; }
        public string Value { get; set; }
    }
}
