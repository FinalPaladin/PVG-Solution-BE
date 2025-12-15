namespace PVG.Infrastucture.Entities
{
    public class ImageRequest : Sample
    {
        public string Url { get; set; }
        public Guid? RequestCode { get; set; }
        public string Content { get; set; }
    }
}