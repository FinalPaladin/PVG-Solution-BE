using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PVG.Infrastucture.Entities
{
    public class User : Sample
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public bool Actived { get; set; }
        public string FullName { get; set; }
    }
}
