using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json.Serialization;

namespace PVG.Domain.Models
{
    public class RQ_SaveRequestCustomerModel
    {
        public Guid? RequestCode { get; set; }
        public string Phone { get; set; }
        public Guid? ProductId { get; set; }
        public string FullName { get; set; }
        [FromForm(Name = "dataJson")]
        public string Data { get; set; }
        public List<ImageRequestCustomerModel> DataImage { get; set; } = new();
    }

    public class SaveRequestCustomerModel
    {
        [JsonPropertyName("key")]
        public string Key { get; set; }
        [JsonPropertyName("value")]
        public string Value { get; set; }
        [JsonPropertyName("name")]
        public string Name { get; set; }
    }

    public class ImageRequestCustomerModel
    {
        public IFormFile ImgFile { get; set; }
    }

    public class RQ_SearchRequestCustomerModel
    {
        public Guid? RequestCode { get; set; }
        public Guid? ProductId { get; set; }
        public string? Phone { get; set; }
        public string FullName { get; set; }
        public bool IsProcessed { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }

    public class RS_GetRequestCustomerModel
    {
        public RequestCustomerModel Data { get; set; } = new();
    }

    public class RequestCustomerDetailModel
    {
        public string Key { get; set; }
        public string Value { get; set; }
    }

    public class RequestCustomerModel
    {
        public Guid? Id { get; set; }
        public Guid? RequestCode { get; set; }
        public string Phone { get; set; }
        public Guid? ProductId { get; set; }
        public string FullName { get; set; }
        public bool IsProcessed { get; set; }
        public bool IsSentEmail { get; set; }
        public string EmailTitle { get; set; }
        public DateTime CreatedDate { get; set; }
        public string? strCreatedDate { 
            get {
                return this.CreatedDate == null ? "" : this.CreatedDate.ToString("yyyy-MM-ddTHH:mm:ss");
            }
        }
        public List<RequestCustomerDetailModel> Details { get; set; } = new();
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