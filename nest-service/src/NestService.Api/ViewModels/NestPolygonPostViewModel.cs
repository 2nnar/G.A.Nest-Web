using System;
using System.Collections.Generic;

namespace NestService.Api.ViewModels
{
    /// <summary>
    /// Nest polygon.
    /// </summary>
    public class NestPolygonPostViewModel
    {
        /// <summary>
        /// Polygon ID.
        /// </summary>
        public Guid Id { get; set; } = Guid.NewGuid();

        /// <summary>
        /// List of polygon vertices.
        /// </summary>
        public IEnumerable<NestPolygonVertexPostViewModel> Vertices { get; set; } = Array.Empty<NestPolygonVertexPostViewModel>();
    }
}
