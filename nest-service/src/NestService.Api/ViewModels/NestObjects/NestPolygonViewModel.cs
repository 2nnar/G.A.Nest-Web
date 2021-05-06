using NestService.Api.ViewModels.Abstract;
using System;
using System.Collections.Generic;

namespace NestService.Api.ViewModels.NestObjects
{
    /// <summary>
    /// Nest polygon.
    /// </summary>
    public class NestPolygonViewModel : NestObjectViewModel
    {
        /// <summary>
        /// List of polygon vertices.s
        /// </summary>
        public IEnumerable<NestObjectVertexViewModel> Vertices { get; set; } = Array.Empty<NestObjectVertexViewModel>();
    }
}
