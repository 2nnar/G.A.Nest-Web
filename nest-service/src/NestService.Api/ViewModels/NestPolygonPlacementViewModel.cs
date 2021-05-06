using System;

namespace NestService.Api.ViewModels
{
    /// <summary>
    /// Nested polygon placement.
    /// </summary>
    public class NestPolygonPlacementViewModel
    {
        /// <summary>
        /// Polygon ID.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Polygon rotation.
        /// </summary>
        public double Rotation { get; set; }

        /// <summary>
        /// Polygon translation point.
        /// </summary>
        public NestPolygonVertexViewModel TranslationPoint { get; set; } = new();
    }
}
