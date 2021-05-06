using NestService.Api.Models.Abstract;
using System;
using System.Collections.Generic;

namespace NestService.Api.Models
{
    public class NestGroup
    {
        public IEnumerable<NestObject> Objects { get; set; } = Array.Empty<NestObject>();
    }
}
