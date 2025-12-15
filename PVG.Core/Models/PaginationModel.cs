namespace PVG.Domain.Models
{
    public class PaginationModel
    {
        public PaginationModel()
        {
            TotalItems = 0;
            TotalPages = 0;
            PerPage = 0;
            PageNumber = 0;
        }
        public bool IsPaging { get; set; } = true;
        public int TotalItems { get; set; }
        public int TotalPages { get; set; }
        public int PerPage { get; set; }
        public int PageNumber { get; set; }
        public object Items { get; set; }
    }

    public class PaginationModel<T>
    {
        public PaginationModel()
        {
            TotalItems = 0;
            TotalPages = 0;
            PerPage = 0;
            PageNumber = 0;
        }

        public bool IsPaging { get; set; } = true;
        public int TotalItems { get; set; }
        public int TotalPages { get; set; }
        public int PerPage { get; set; }
        public int PageNumber { get; set; }
        public T Items { get; set; }
    }
}