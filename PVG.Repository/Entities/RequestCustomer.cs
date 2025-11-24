namespace PVG.Infrastucture.Entities
{
    public class RequestCustomer : Sample
    {
        public Guid? Id { get; set; }
        public Guid? RequestCode { get; set; }
        public string Phone { get; set; }
        public string Key { get; set; }
        public string Value { get; set; }
        public Guid? ProductId { get; set; }
    }
}