namespace PVG.Domain.Models
{
    public class RQ_SaveRequestCustomerModel
    {
        public Guid? RequestCode { get; set; }
        public string Phone { get; set; }
        public Guid? ProductId { get; set; }
        public List<SaveRequestCustomerModel> Data { get; set; } = new();
    }

    public class SaveRequestCustomerModel
    {
        public string Key { get; set; }
        public string Value { get; set; }
    }

    public class RQ_SearchRequestCustomerModel
    {
        public Guid? RequestCode { get; set; }
        public Guid? ProductId { get; set; }
        public string? Phone { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }

    public class RS_GetRequestCustomerModel
    {
        public RequestCustomerModel Data { get; set; } = new();
    }

    public class GetRequestCustomerModel
    {
        public Guid? RequestCode { get; set; }
        public string Phone { get; set; }
        public Guid? ProductId { get; set; }
        public string? CreatedDate { get; set; }
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
        public string Key { get; set; }
        public string Value { get; set; }
        public Guid? ProductId { get; set; }
        public DateTime CreatedDate { get; set; }
        public string? strCreatedDate { 
            get {
                return this.CreatedDate == null ? "" : this.CreatedDate.ToString("yyyy-MM-ddTHH:mm:ss");
            }
        }
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