namespace PVG.Infrastucture.Entities
{
    public class RequestCustomer : Sample
    {
        public Guid? RequestCode { get; set; }
        public string Phone { get; set; }
        public Guid? ProductId { get; set; }
        public string FullName { get; set; }
        public bool IsProcessed { get; set; }
    }
}