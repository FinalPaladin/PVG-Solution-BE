namespace PVG.Infrastucture.Entities
{
    public class Product : Sample
    {
        public Guid? ProductCategoryId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Image { get; set; }
    }
}