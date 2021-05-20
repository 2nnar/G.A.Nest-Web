using System;
using System.Collections.Generic;
using System.Linq;

namespace NestService.Api.Models
{
    public class NestGroup : NestObject
    {
        public IEnumerable<NestObject> Objects { get; set; } = Array.Empty<NestObject>();
        public override NestObjectPoint MaxPoint
        {
            get
            {
                var maxX = Objects.Select(o => o.MaxPoint).Max(p => p.X);
                var maxY = Objects.Select(o => o.MaxPoint).Max(p => p.Y);
                return new(maxX, maxY);
            }
        }
        public override NestObjectPoint MinPoint
        {
            get
            {
                var minX = Objects.Select(o => o.MinPoint).Max(p => p.X);
                var minY = Objects.Select(o => o.MinPoint).Max(p => p.Y);
                return new(minX, minY);
            }
        }
    }
}
