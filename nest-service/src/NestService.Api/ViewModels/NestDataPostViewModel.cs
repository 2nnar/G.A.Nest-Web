using NestService.Api.Configuration.Converters;
using NestService.Api.ViewModels.Abstract;
using NestService.Api.ViewModels.NestObjects;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace NestService.Api.ViewModels
{
    /// <summary>
    /// Nest data.
    /// </summary>
    public class NestDataPostViewModel
    {
        /// <summary>
        /// List of objects to nest.
        /// </summary>
        [JsonProperty(ItemConverterType = typeof(NestObjectConverter))]
        public IEnumerable<NestObjectViewModel> Objects { get; set; } = Array.Empty<NestObjectViewModel>();

        /// <summary>
        /// Bin.
        /// </summary>
        public NestObjectViewModel Bin { get; set; } = new NestPolygonViewModel();
    }
}
