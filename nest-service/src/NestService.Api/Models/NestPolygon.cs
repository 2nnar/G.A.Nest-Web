using System;
using System.Collections.Generic;
using System.Linq;

namespace NestService.Api.Models
{
    public class NestPolygon : NestObject
    {
        public IEnumerable<NestObjectPoint> Vertices { get; set; } = Array.Empty<NestObjectPoint>();
        public override NestObjectPoint MaxPoint => new(Vertices.Max(v => v.X), Vertices.Max(v => v.Y));
        public override NestObjectPoint MinPoint => new(Vertices.Min(v => v.X), Vertices.Min(v => v.Y));
    }
}
