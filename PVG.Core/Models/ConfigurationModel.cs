using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace PVG.Domain.Models
{
    public class RS_GetAllConfigurationModel
    {
        public List<ConfigurationModel> Data { get; set; } = new();
    }

    public class ConfigurationModel
    {
        [JsonPropertyName("key")]
        public string Key { get; set; }
        [JsonPropertyName("value")]
        public string Value { get; set; }
    }

    public class ImageConfigurationModel
    {
        public string Key { get; set; }
        public IFormFile ImgFile { get; set; }
    }

    public class RQ_SaveConfigurationModel
    {
        public string CreateUser { get; set; }
        [FromForm(Name = "dataJson")]
        public string Data { get; set; } 
        public List<ImageConfigurationModel> DataImage { get; set; } = new();
    }

}
