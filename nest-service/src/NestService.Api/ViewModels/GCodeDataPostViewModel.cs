using NestService.Api.Configuration.Converters;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace NestService.Api.ViewModels
{
    /// <summary>
    /// Data for G-code generation.
    /// </summary>
    public class GCodeDataPostViewModel
    {
        /// <summary>
        /// List of nested objects.
        /// </summary>
        [JsonProperty(ItemConverterType = typeof(NestObjectConverter))]
        public IEnumerable<NestObjectViewModel> Objects { get; set; } = Array.Empty<NestObjectViewModel>();
    }
}
