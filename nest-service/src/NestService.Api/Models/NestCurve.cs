using NestService.Api.Models.Abstract;
using System;

namespace NestService.Api.Models
{
    public class NestCurve : NestObject
    {
        public NestObjectPoint Center { get; set; } = new();
        public double MajorRadius { get; set; }
        public double MinorRadius { get; set; }
        public double StartParam { get; set; }
        public double EndParam { get; set; }
        public override NestObjectPoint MaxPoint => new(Center.X + MajorRadius, Center.Y + MinorRadius);
        public override NestObjectPoint MinPoint => new(Center.X - MajorRadius, Center.Y - MinorRadius);

        public NestObjectPoint GetPointAtParameter(double param)
            => new(
                Center.X + MajorRadius * Math.Cos(param),
                Center.Y + MinorRadius * Math.Sin(param));
    }
}
