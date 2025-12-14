namespace PVG.Infrastucture.Entities
{
    public class ProductDetail : Sample
    {
        public Guid? ProductId { get; set; }
        public Guid? ProductDetailCategoryId { get; set; }
        public string Content { get; set; }
    }
}