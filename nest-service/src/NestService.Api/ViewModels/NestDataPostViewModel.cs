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
        /// List of polygons to nest.
        /// </summary>
        public IEnumerable<NestPolygonPostViewModel> Polygons { get; set; } = Array.Empty<NestPolygonPostViewModel>();

        /// <summary>
        /// Bin.
        /// </summary>
        public NestPolygonPostViewModel Bin {get;set;} = new();
    }
}
