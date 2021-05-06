using NestService.Api.Models.Abstract;

namespace NestService.Api.Models
{
    public class NestCurve : NestObject
    {
        public NestObjectVertex Center { get; set; } = new();
        public double MajorRadius { get; set; }
        public double MinorRadius { get; set; }
        public double StartParam { get; set; }
        public double EndParam { get; set; }
    }
}
