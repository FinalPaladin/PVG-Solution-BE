using static PVG.Domain.Enums.ViewLogEnum;

namespace PVG.Infrastucture.Entities
{
    public class ViewLog : Sample
    {
        public string IP { get; set; }
        public int NumberOfTimes { get; set; }
        public ScreenView Screen { get; set; }
        public Guid? DetailId { get; set; }
    }
}