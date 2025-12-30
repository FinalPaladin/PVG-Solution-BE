namespace PVG.Domain.BaseModels
{
    public class RequestPagingModel
    {
        public bool IsPaging { get; set; } = true;
        public int Page { get; set; }
        public int PageSize { get; set; }
    }
}