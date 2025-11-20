using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PVG.Domain.Models
{
    public class RS_GetAllConfigurationModel
    {
        public List<ConfigurationModel> Data { get; set; } = new();
    }

    public class ConfigurationModel
    {
        public string Key { get; set; }
        public string Value { get; set; }
    }

    public class RQ_SaveConfigurationModel
    {
        public Guid? CreateUserId { get; set; }
        public List<ConfigurationModel> Data { get; set; } = new();
    }

}
