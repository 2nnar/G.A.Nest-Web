using NestService.Api.Models.Abstract;
using System;
using System.Collections.Generic;

namespace NestService.Api.Models
{
    public class NestPolygon : NestObject
    {
        public IEnumerable<NestObjectVertex> Vertices { get; set; } = Array.Empty<NestObjectVertex>();
    }
}
